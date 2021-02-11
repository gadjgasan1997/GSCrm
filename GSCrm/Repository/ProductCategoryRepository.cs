using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Mapping;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using static GSCrm.Utils.CollectionsUtils;
using static GSCrm.CommonConsts;

namespace GSCrm.Repository
{
    public class ProductCategoryRepository : BaseRepository<ProductCategory, ProductCategoryViewModel>
    {
        #region Declarations
        /// <summary>
        /// Минимальная длина названия категории
        /// </summary>
        private const int MIN_PROD_CAT_NAME_LENGTH = 3;
        /// <summary>
        /// Максимальная длина названия категории
        /// </summary>
        private const int MAX_PROD_CAT_NAME_LENGTH = 100;
        /// <summary>
        /// Максимальная длина описания категории
        /// </summary>
        private const int MAX_PROD_CAT_DESC_LENGTH = 2000;
        #endregion

        public ProductCategoryRepository(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        #region Override Methods
        public override bool HasPermissionsForSeeItem(ProductCategory productCategory)
            => new OrganizationRepository(serviceProvider, context).HasPermissionsForSeeOrgItem();

        protected override bool RespsIsCorrectOnCreate(ProductCategoryViewModel entityToCreate)
        {
            return true;
        }

        protected override bool TryCreatePrepare(ProductCategoryViewModel prodCatViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(prodCatViewModel),
                () => CheckCategoryNotExistsOnCreate(prodCatViewModel)
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnUpdate(ProductCategoryViewModel entityToUpdate)
        {
            return true;
        }

        protected override bool TryUpdatePrepare(ProductCategoryViewModel prodCatViewModel)
        {
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => CommonChecks(prodCatViewModel),
                () => CheckCategoryNotExistsOnUpdate(prodCatViewModel)
            });
            return !errors.Any();
        }

        protected override bool RespsIsCorrectOnDelete(ProductCategory entityToDelete)
        {
            return true;
        }
        #endregion

        #region Attach Categories
        public void AttachProductCategories(ref ProductCategoriesViewModel prodCatsViewModel)
        {
            ProductCategoriesViewModel viewModel = prodCatsViewModel;
            viewModel.ProductCategoryViewModels = context.GetOrgProdCats(prodCatsViewModel.OrganizationViewModel.Id)
                .MapToViewModels(map, prodCats => GetLimitedProdCatsList(prodCats, viewModel));
            prodCatsViewModel = viewModel;
        }

        private List<ProductCategory> GetLimitedProdCatsList(List<ProductCategory> allProdCats, ProductCategoriesViewModel prodCatsViewModel)
        {
            LimitByCategoryName(ref allProdCats, prodCatsViewModel);
            LimitByPageNumber(ref allProdCats);
            LimitProducts(ref allProdCats, prodCatsViewModel);
            return allProdCats.OrderBy(n => n.Name).ToList();
        }

        /// <summary>
        /// Ограничение списка категорий продуктов по названию
        /// </summary>
        /// <param name="allProdCats"></param>
        /// <param name="prodCatsViewModel"></param>
        private void LimitByCategoryName(ref List<ProductCategory> allProdCats, ProductCategoriesViewModel prodCatsViewModel)
        {
            // Применение фильтрации если необходимо
            string searchSpec = prodCatsViewModel.SearchProductCategoryName?.ToLower().TrimStartAndEnd();
            if (!string.IsNullOrEmpty(searchSpec))
            {
                List<ProductCategory> limitedProdCats = new List<ProductCategory>();

                // Директории, не являющиеся корневыми
                List<ProductCategory> nonRootProdCats = allProdCats.Where(prodCat => prodCat.ParentProductCategoryId != null).ToList();

                // Получение списка категорий, у которых нет дочерних
                List<ProductCategory> hasNoChildsProdCats = new List<ProductCategory>();
                foreach(ProductCategory nonRootProdCat in nonRootProdCats)
                {
                    ProductCategory parentProdCat = allProdCats.FirstOrDefault(cat => cat.ParentProductCategoryId == nonRootProdCat.Id);
                    if (parentProdCat == null)
                        hasNoChildsProdCats.Add(nonRootProdCat);
                }

                // Для всего этого списка запускается поиск подходящих категорий
                foreach (ProductCategory hasNoChildsProdCat in hasNoChildsProdCats)
                {
                    if (hasNoChildsProdCat.Name.ToLower().Contains(searchSpec))
                        AddHierarchyToResultList(hasNoChildsProdCat, allProdCats, limitedProdCats);
                    else
                    {
                        ProductCategory parentProdCat = allProdCats.FirstOrDefault(i => i.Id == hasNoChildsProdCat.ParentProductCategoryId);
                        while (parentProdCat != null)
                        {
                            if (parentProdCat.Name.ToLower().Contains(searchSpec))
                            {
                                AddHierarchyToResultList(parentProdCat, allProdCats, limitedProdCats);
                                break;
                            }
                            if (parentProdCat.ParentProductCategoryId == null)
                                break;
                            parentProdCat = allProdCats.FirstOrDefault(i => i.Id == parentProdCat.ParentProductCategoryId);
                        }
                    }
                }

                // Директории, являющиеся корневыми
                List<ProductCategory> rootProdCats = allProdCats.Where(prodCat => prodCat.ParentProductCategoryId == null).ToList();
                foreach (ProductCategory rootProdCat in rootProdCats)
                {
                    // Если категория уже не была добавлена в список
                    if (!limitedProdCats.Contains(rootProdCat, new ProductCategoryEqualityComparer()))
                    {
                        if (!string.IsNullOrEmpty(searchSpec))
                        {
                            if (rootProdCat.Name.ToLower().Contains(searchSpec))
                                limitedProdCats.Add(rootProdCat);
                        }
                        else limitedProdCats.Add(rootProdCat);
                    }
                }
                allProdCats = limitedProdCats;
            }
        }

        /// <summary>
        /// Ограничение списка категорий продуктов по номеру страницы
        /// </summary>
        /// <param name="allProdCats"></param>
        private void LimitByPageNumber(ref List<ProductCategory> allProdCats)
        {
            // Поиск корневых директорий и ограничение их по количеству отображаемых
            List<ProductCategory> rootProdCats = allProdCats.Where(prodCat => prodCat.ParentProductCategoryId == null).ToList();
            LimitListByPageNumber(PROD_CATS, ref rootProdCats);

            // В результате должны остаться только категории, являющиеся дочерними по отношению к отобранным корневым
            Func<ProductCategory, bool> predicate = prodCat => prodCat.RootCategoryId != null &&
                rootProdCats.Select(root => root.Id).ToList().Contains((Guid)prodCat.RootCategoryId);
            allProdCats = rootProdCats.Concat(allProdCats.Where(predicate)).ToList();
        }

        /// <summary>
        /// Метод добавляет поданный на вход элемент и всю его иерархию в результирующий список
        /// </summary>
        /// <param name="currentProdCat">Текущая категория</param>
        /// <param name="allProdCats">Все категории</param>
        /// <param name="resultList">Результирующий список</param>
        private void AddHierarchyToResultList(ProductCategory currentProdCat, List<ProductCategory> allProdCats, List<ProductCategory> resultList)
        {
            do
            {
                if (resultList.Contains(currentProdCat))
                    return;
                resultList.Add(currentProdCat);
                if (currentProdCat.ParentProductCategoryId == null)
                    return;
                currentProdCat = allProdCats.FirstOrDefault(i => i.Id == currentProdCat.ParentProductCategoryId);
            }
            while (currentProdCat != null);
        }

        /// <summary>
        /// Ограничение списка продуктов в уже отфильтрованном списке категорий
        /// </summary>
        /// <param name="allProdCats"></param>
        /// <param name="prodCatsViewModel"></param>
        private void LimitProducts(ref List<ProductCategory> allProdCats, ProductCategoriesViewModel prodCatsViewModel)
        {
            // Для каждой категории необходимо получить список продуктов
            ProductMap productMap = new ProductMap(serviceProvider, context);
            ProductRepository productRepository = new ProductRepository(serviceProvider, context);
            allProdCats.ForEach(productCategory =>
            {
                // Фильтрация списка продуктов теми, которые находятся в отфильтрованном списке категорий
                // Затем применяются продуктовые фильтры
                List<ProductViewModel> productViewModels = productCategory.GetProducts(context)
                    .MapToViewModels(productMap, products =>
                        productRepository.GetLimitedProductList(products, prodCatsViewModel));
                if (productViewModels.Count > 0)
                {
                    string categoryId = productCategory.Id.ToString();
                    if (!prodCatsViewModel.CategoriesProducts.ContainsKey(categoryId))
                        prodCatsViewModel.CategoriesProducts.Add(categoryId, productViewModels);
                    else prodCatsViewModel.CategoriesProducts[categoryId] = productViewModels;
                }
            });
        }
        #endregion

        #region Searching
        /// <summary>
        /// Метод выполняет поиск по продуктам и категориям
        /// </summary>
        public void Search(ProductCategoriesViewModel prodCatsViewModel)
        {
            if (cachService.TryGetEntityCache(currentUser, out ProductCategoriesViewModel prodCatsCache, PROD_CATS))
            {
                prodCatsCache.SearchProductCategoryName = prodCatsViewModel.SearchProductCategoryName;
                prodCatsCache.SearchProductName = prodCatsViewModel.SearchProductName;
                prodCatsCache.MinConst = prodCatsViewModel.MinConst;
                prodCatsCache.MaxConst = prodCatsViewModel.MaxConst;
                cachService.AddOrUpdate(currentUser, PROD_CATS, prodCatsCache);
            }
        }

        /// <summary>
        /// Метод очищает поиск по продуктам и категориям
        /// </summary>
        public void ClearSearch()
        {
            if (cachService.TryGetEntityCache(currentUser, out ProductCategoriesViewModel prodCatsCache, PROD_CATS))
            {
                prodCatsCache.SearchProductCategoryName = default;
                prodCatsCache.SearchProductName = default;
                prodCatsCache.MinConst = default;
                prodCatsCache.MaxConst = default;
                cachService.AddOrUpdate(currentUser, PROD_CATS, prodCatsCache);
            }
        }
        #endregion

        #region Validations
        /// <summary>
        /// Метод проверяет, что не существует категории с таким же названием при создании записи
        /// </summary>
        /// <param name="prodCatViewModel"></param>
        private void CheckCategoryNotExistsOnCreate(ProductCategoryViewModel prodCatViewModel)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            List<ProductCategory> productCategories = currentOrganization.GetProductCategories(context);

            // Проверка в случае, если создается корневая директория
            if (string.IsNullOrEmpty(prodCatViewModel.ParentProductCategoryId))
            {
                if (productCategories.Where(p => p.ParentProductCategoryId == null).Select(n => n.Name.ToLower()).Contains(prodCatViewModel.Name.ToLower()))
                    errors.Add("CategoryAlreadyExists", resManager.GetString("CategoryAlreadyExists"));
            }

            // Если создается дочерняя категория
            else if (TryGetParentCategory(prodCatViewModel, productCategories, out ProductCategory parentProductCategory))
            {
                if (productCategories.Where(p => p.ParentProductCategoryId == parentProductCategory.Id).Select(n => n.Name.ToLower()).Contains(prodCatViewModel.Name.ToLower()))
                    errors.Add("CategoryAlreadyExists", resManager.GetString("CategoryAlreadyExists"));
            }
        }

        /// <summary>
        /// Метод проверяет, что не существует категории с таким же названием при изменении записи
        /// </summary>
        /// <param name="prodCatViewModel"></param>
        private void CheckCategoryNotExistsOnUpdate(ProductCategoryViewModel prodCatViewModel)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            List<ProductCategory> productCategories = currentOrganization.GetProductCategories(context);
            ProductCategory changedProductCategory = (ProductCategory)transaction.GetParameterValue("RecordToChange");

            // Проверка в случае, если изменяется корневая директория
            if (changedProductCategory.ParentProductCategoryId == null)
            {
                Func<ProductCategory, bool> predicate = p => p.ParentProductCategoryId == null && p.Id != changedProductCategory.Id;
                if (productCategories.Where(predicate).Select(n => n.Name.ToLower()).Contains(prodCatViewModel.Name.ToLower()))
                    errors.Add("CategoryAlreadyExists", resManager.GetString("CategoryAlreadyExists"));
            }

            // Если изменяется дочерняя категория
            else
            {
                ProductCategory parentProductCategory = context.ProductCategories.AsNoTracking().FirstOrDefault(i => i.Id == changedProductCategory.ParentProductCategoryId);
                if (parentProductCategory == null)
                    errors.Add("ParentCategoryNotFound", resManager.GetString("ParentCategoryNotFound"));
                else
                {
                    Func<ProductCategory, bool> predicate = p => p.ParentProductCategoryId == parentProductCategory.Id && p.Id != changedProductCategory.Id;
                    if (productCategories.Where(predicate).Select(n => n.Name.ToLower()).Contains(prodCatViewModel.Name.ToLower()))
                        errors.Add("CategoryAlreadyExists", resManager.GetString("CategoryAlreadyExists"));
                }
            }
        }

        /// <summary>
        /// Метод пытается найти родительскую категорию
        /// </summary>
        /// <param name="prodCatViewModel"></param>
        /// <param name="productCategories"></param>
        /// <param name="parentProductCategory"></param>
        /// <returns></returns>
        private bool TryGetParentCategory(ProductCategoryViewModel prodCatViewModel, List<ProductCategory> productCategories, out ProductCategory parentProductCategory)
        {
            // Попытка распарсить id
            parentProductCategory = null;
            if (!Guid.TryParse(prodCatViewModel.ParentProductCategoryId, out Guid parentProductCategoryId))
            {
                errors.Add("ParentCategoryNotFound", resManager.GetString("ParentCategoryNotFound"));
                return false;
            }

            // Попытка найти категорию
            ProductCategory parentProdCat = productCategories.FirstOrDefault(i => i.Id == parentProductCategoryId);
            if (parentProdCat == null)
            {
                errors.Add("ParentCategoryNotFound", resManager.GetString("ParentCategoryNotFound"));
                return false;
            }

            parentProductCategory = parentProdCat;
            transaction.AddParameter("ParentProductCategory", parentProdCat);
            return true;
        }

        private void CommonChecks(ProductCategoryViewModel prodCatViewModel)
        {
            Organization currentOrganization = (Organization)transaction.GetParameterValue("CurrentOrganization");
            prodCatViewModel.Normalize();
            InvokeIntermittinActions(errors, new List<Action>()
            {
                () => {
                    if (currentOrganization == null)
                        errors.Add("OrganizationNotFound", resManager.GetString("OrganizationNotFound"));
                },
                () => {
                    if (string.IsNullOrEmpty(prodCatViewModel.Name) || prodCatViewModel.Name.Length < MIN_PROD_CAT_NAME_LENGTH)
                        errors.Add("ProductCategoryLength", resManager.GetString("ProductCategoryLength")
                            .Replace("{min}", MIN_PROD_CAT_NAME_LENGTH.ToString())
                            .Replace("{max}", MAX_PROD_CAT_NAME_LENGTH.ToString()));
                },
                () => {
                    if (!string.IsNullOrEmpty(prodCatViewModel.Description) && prodCatViewModel.Description.Length > MAX_PROD_CAT_DESC_LENGTH)
                        errors.Add("CategoryDescriptionLength", resManager.GetString("CategoryDescriptionLength").Replace("{n}", MAX_PROD_CAT_DESC_LENGTH.ToString()));
                }
            });
        }
        #endregion
    }
}
