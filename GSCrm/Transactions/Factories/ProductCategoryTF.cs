using System;
using System.Collections.Generic;
using System.Linq;
using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GSCrm.Transactions.Factories
{
    public class ProductCategoryTF : TransactionFactory<ProductCategoryViewModel>
    {
        public ProductCategoryTF(IServiceProvider serviceProvider, ApplicationDbContext context) : base(serviceProvider, context)
        { }

        protected override void BeforeCommit(OperationType operationType)
        {
            if (operationType == OperationType.Delete)
            {
                Organization currentOrganization = cachService.GetCachedCurrentEntity<Organization>(currentUser);
                ProductCategory productCategory = (ProductCategory)transaction.GetParameterValue("RecordToRemove");
                List<ProductCategory> allProductCategories = currentOrganization.GetProductCategories(context);
                DeleteChildCategories(productCategory, allProductCategories);
            }
        }

        /// <summary>
        /// Метод удаляет дочерние для поданной на вход категории
        /// </summary>
        /// <param name="productCategory"></param>
        /// <param name="allProductCategories">Список всех категорий организации</param>
        private void DeleteChildCategories(ProductCategory productCategory, List<ProductCategory> allProductCategories)
        {
            List<ProductCategory> childCategories = allProductCategories.Where(p => p.ParentProductCategoryId == productCategory.Id).ToList();
            childCategories.ForEach(childCategory =>
            {
                transaction.AddChange(childCategory, EntityState.Deleted);
                DeleteChildCategories(childCategory, allProductCategories);
            });
        }
    }
}
