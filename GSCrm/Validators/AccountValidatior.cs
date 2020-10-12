using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using GSCrm.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static GSCrm.RegexConsts;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class AccountValidatior : BaseValidator<AccountViewModel>
    {
        // Длины имени, фамилия и отчества
        private const int FIRST_NAME_MIN_LENGTH = 2;
        private const int FIRST_NAME_MAX_LENGTH = 300;
        private const int LAST_NAME_MIN_LENGTH = 2;
        private const int LAST_NAME_MAX_LENGTH = 300;
        private const int MIDDLE_NAME_MAX_LENGTH = 300;

        // Возможная длина ИНН
        private const int INN_TEN_LENGTH = 10;
        private const int INN_TWELVE_LENGTH = 12;

        // Множители цифр для ИНН
        private static readonly int[] INN_10_NUM_FACTORS = { 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        private static readonly int[] INN_12_NUM_FACTORS_FIRST_CHECK = { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };
        private static readonly int[] INN_12_NUM_FACTORS_LAST_CHECK = { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8 };

        // Делитель для ИНН
        private const int INN_DIVIDER = 11;

        // Возможная длина КПП
        private const int KPP_LENGTH = 9;

        // Возможная длина ОКПО
        private const int OKPO_LENGTH = 8;

        // Множители цифр для ОКПО
        private static readonly int[] OKPO_NUM_FACTORS_FIRST_CHECK = { 1, 2, 3, 4, 5, 6, 7 };
        private static readonly int[] OKPO_NUM_FACTORS_LAST_CHECK = { 3, 4, 5, 6, 7, 8, 9 };

        // Делитель для ОКПО
        private const int OKPO_DIVIDER = 11;

        // Возможная длина ОГРН
        private const int OGRN_LENGTH = 13;

        // Делитель для ОГРН
        private const int OGRN_DIVIDER = 11;

        private readonly User currentUser;

        public AccountValidatior(ApplicationDbContext context, ResManager resManager, HttpContext httpContext = null) : base(context, resManager)
        {
            if (httpContext != null)
                currentUser = context.Users.FirstOrDefault(n => n.UserName == httpContext.User.Identity.Name);
        }

        public override Dictionary<string, string> CreationCheck(AccountViewModel accountViewModel)
        {
            if (!TryCheckAccountType(accountViewModel, out AccountType accountType))
                return errors;
            else
            {
                List<Action> commonHandlers = new List<Action>()
                {
                    () => CheckCountry(accountViewModel),
                    () => CheckPrimaryManager(accountViewModel)
                };
                
                switch (accountType)
                {
                    case AccountType.Individual:
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckIndividualFullName(accountViewModel),
                            () => CheckINNOnCreate(accountViewModel)
                        }.Concat(commonHandlers));
                        break;

                    case AccountType.IndividualEntrepreneur:
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckIENameOnCreate(accountViewModel),
                            () => CheckINNOnCreate(accountViewModel)
                        }.Concat(commonHandlers));
                        break;

                    case AccountType.LegalEntity:
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckINNOnCreate(accountViewModel),
                            () => CheckLENameOnCreate(accountViewModel),
                            () => CheckKPPOnCreate(accountViewModel),
                            () => CheckOKPOOnCreate(accountViewModel),
                            () => CheckOGRNOnCreate(accountViewModel)
                        }.Concat(commonHandlers));
                        break;
                }
            }
            return errors;
        }

        public override Dictionary<string, string> UpdateCheck(AccountViewModel accountViewModel)
        {
            if (!TryCheckAccountType(accountViewModel, out AccountType accountType))
                return errors;
            else
            {
                Account account = context.Accounts.FirstOrDefault(i => i.Id == accountViewModel.Id);
                InvokeIntermittingChecks(errors, new List<Action>()
                {
                    () => CheckAccountTypeNotChanged(accountViewModel, account),
                    () => CheckManagerExists(account)
                });
                if (errors.Any()) return errors;
                switch (accountType)
                {
                    case AccountType.Individual:
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckIndividualNameNotChanged(accountViewModel, account),
                            () => CheckINNOnUpdate(accountViewModel)
                        });
                        break;

                    case AccountType.IndividualEntrepreneur:
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckIENameOnUpdate(accountViewModel),
                            () => CheckINNOnUpdate(accountViewModel)
                        });
                        break;

                    case AccountType.LegalEntity:
                        InvokeAllChecks(new List<Action>()
                        {
                            () => CheckLENameOnUpdate(accountViewModel),
                            () => CheckINNOnUpdate(accountViewModel),
                            () => CheckKPPOnUpdate(accountViewModel),
                            () => CheckOKPOOnUpdate(accountViewModel),
                            () => CheckOGRNOnUpdate(accountViewModel)
                        });
                        break;
                }
            }
            return errors;
        }

        /// <summary>
        /// Метод выполняет проверки, необходимые при изенении основного контакта на клиенте
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <returns></returns>
        public Dictionary<string, string> ChangePrimaryContactCheck(AccountViewModel accountViewModel, Account account)
        {
            // Для физических лиц основной контакт является обязательным
            if (account.AccountType == AccountType.Individual && string.IsNullOrEmpty(accountViewModel.PrimaryContactId))
            {
                errors.Add("PrimaryIndividualContactIsRequired", resManager.GetString("PrimaryIndividualContactIsRequired"));
                return errors;
            }

            // Для остальных типов клиентов основной контакт не является обязательным, поэтому осуществляется возврат из метода
            if (account.AccountType != AccountType.Individual && string.IsNullOrEmpty(accountViewModel.PrimaryContactId))
                return errors;

            // Если не получается распарсить строку с контактом, возвращается ошибка
            if (!Guid.TryParse(accountViewModel.PrimaryContactId, out Guid primaryContactId))
            {
                errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));
                return errors;
            }

            // Если контак не найден, также возвращается ошибка
            AccountContact accountContact = account.AccountContacts.FirstOrDefault(i => i.Id == primaryContactId);
            if (accountContact == null)
            {
                errors.Add("AccountContactNotFound", resManager.GetString("AccountContactNotFound"));
                return errors;
            }
            return errors;
        }

        /// <summary>
        /// Метод производит проверки при изменении юридического адреса клиента
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public Dictionary<string, string> ChangeLegalAddressCheck(AccountAddressViewModel addressViewModel, Account account)
        {
            if (!Enum.TryParse(typeof(AddressType), addressViewModel.CurrentAddressNewType, out object newAddressType))
            {
                errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                return errors;
            }

            if ((AddressType)newAddressType == AddressType.Legal)
            {
                errors.Add("AddressTypeWrong", resManager.GetString("AddressTypeWrong"));
                return errors;
            }

            if (!Guid.TryParse(addressViewModel.NewLegalAddressId, out Guid newLegalAddressId))
            {
                errors.Add("AddressNotFound", resManager.GetString("AddressNotFound"));
                return errors;
            }

            if (context.AccountAddresses.FirstOrDefault(i => i.Id == newLegalAddressId) == null)
                errors.Add("AddressNotFound", resManager.GetString("AddressNotFound"));
            return errors;
        }

        /// <summary>
        /// Проверка на то, что тип клиента является допустимым
        /// </summary>
        /// <param name="accountViewModel"></param>
        private bool TryCheckAccountType(AccountViewModel accountViewModel, out AccountType accountType)
        {
            if (!Enum.TryParse(typeof(AccountType), accountViewModel.AccountType, out object type))
            {
                errors.Add("WrongAccountType", resManager.GetString("WrongAccountType"));
                accountType = AccountType.None;
                return false;
            }
            accountType = (AccountType)type;
            return true;
        }

        /// <summary>
        /// Проверяет фамилию, имя и отчество физического лица
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIndividualFullName(AccountViewModel accountViewModel)
            => new PersonValidator(resManager).CheckPersonName(accountViewModel.FirstName, accountViewModel.LastName, accountViewModel.MiddleName, ref errors);

        /// <summary>
        /// Проверка ИНН при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckINNOnCreate(AccountViewModel accountViewModel)
        {
            string inn = accountViewModel.INN?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckINN(inn),
                () => CheckINNUnique(inn)
            });
        }

        /// <summary>
        /// Проверка ИНН при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckINNOnUpdate(AccountViewModel accountViewModel)
        {
            string inn = accountViewModel.INN?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckINN(inn),
                () => CheckINNUnique(inn, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ИНН на корректность
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINN(string inn)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckINNLength(inn),
                () => CheckINNSymbols(inn),
                () =>
                {
                    if (inn.Length == INN_TEN_LENGTH)
                        CheckTenCharactersINN(inn);
                    else CheckTwelveCharactersINN(inn);
                }
            });
        }

        /// <summary>
        /// Проверка ИНН на длину
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINNLength(string inn)
        {
            if (string.IsNullOrEmpty(inn) || (inn.Length != INN_TEN_LENGTH && inn.Length != INN_TWELVE_LENGTH))
                errors.Add("INNLength", resManager.GetString("INNLength"));
        }

        /// <summary>
        /// Проверка ИНН на недопустимые символы
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINNSymbols(string inn)
        {
            if (ONLY_DIGITS.IsMatch(inn))
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
        }

        /// <summary>
        /// Проверка ИНН при длине в 10 символов 
        /// </summary>
        /// <param name="inn"></param>
        private void CheckTenCharactersINN(string inn)
        {
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int numeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH - 1; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                numeralSumm += Convert.ToInt32(numeral) * INN_10_NUM_FACTORS[numeralIndex];
            }

            // Контрольная сумма
            int checkNumber = GetCheckNumber(numeralSumm);

            // Сравнение контрольной суммы с 10 цифрой в ИНН
            string lastNumeral = inn[INN_TEN_LENGTH - 1].ToString();
            if (checkNumber != Convert.ToInt32(lastNumeral))
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
        }

        /// <summary>
        /// Проверка ИНН при длине в 12 символов 
        /// </summary>
        /// <param name="inn"></param>
        private void CheckTwelveCharactersINN(string inn)
        {
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int firstNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                firstNumeralSumm += Convert.ToInt32(numeral) * INN_12_NUM_FACTORS_FIRST_CHECK[numeralIndex];
            }

            int firstCheckNumber = GetCheckNumber(firstNumeralSumm);

            // Проверка на равенство первого контрольного числа с 11-й цифрой ИНН
            string elevenINNNumeral = inn[INN_TEN_LENGTH].ToString();
            if (firstCheckNumber != Convert.ToInt32(elevenINNNumeral))
            {
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
                return;
            }

            // Если проверка пройдена, вычисляется следующее контрольное число
            // Нахождение суммы произведений цифр ИНН на соответствующий множитель
            int lastNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < INN_TEN_LENGTH + 1; numeralIndex++)
            {
                string numeral = inn[numeralIndex].ToString();
                lastNumeralSumm += Convert.ToInt32(numeral) * INN_12_NUM_FACTORS_LAST_CHECK[numeralIndex];
            }

            // Второе контрольное число
            int lastCheckNumber = GetCheckNumber(lastNumeralSumm);

            // Проверка на равенство второго контрольного числа с 12-й цифрой ИНН
            string twelveINNNumeral = inn[INN_TEN_LENGTH + 1].ToString();
            if (lastCheckNumber != Convert.ToInt32(twelveINNNumeral))
                errors.Add("INNWrong", resManager.GetString("INNWrong"));
        }

        /// <summary>
        /// Метод вычисляет контрольное число
        /// </summary>
        /// <param name="numeralSumm">Сумма произведений цифр ИНН на соответствующие множители</param>
        /// <returns></returns>
        private int GetCheckNumber(int numeralSumm)
        {
            // Число "numeralSumm" делится на константу INN_DIVIDER для получения целой части, затем умножается на нее
            int numeralQuotient = numeralSumm / INN_DIVIDER;
            int numeralComposition = numeralQuotient * INN_DIVIDER;

            // Получение и возврат контрольной суммы
            int checkNumber = numeralSumm - numeralComposition;
            if (checkNumber < 0)
                checkNumber *= (-1);
            if (checkNumber == 10)
                checkNumber = 0;
            return checkNumber;
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ИНН (используется при создании)
        /// </summary>
        /// <param name="inn"></param>
        private void CheckINNUnique(string inn)
        {
            Account accountWithSameINN = context.GetOrgAccounts(currentUser.PrimaryOrganizationId).FirstOrDefault(i => i.INN == inn);
            if (accountWithSameINN != null)
                errors.Add("INNNotUnique", resManager.GetString("INNNotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ИНН (используется при обновлении, так как необходимо исключить инн обновляемого клиента из поиска)
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="updatedAccount">Клиент, инн которого обновляется</param>
        private void CheckINNUnique(string inn, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameINN = orgAccounts.FirstOrDefault(i => i.INN == inn && i.Id != updatedAccount.Id);
            if (accountWithSameINN != null)
                errors.Add("INNNotUnique", resManager.GetString("INNNotUnique"));
        }

        /// <summary>
        /// Проверка КПП при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckKPPOnCreate(AccountViewModel accountViewModel)
        {
            string kpp = accountViewModel.KPP?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckKPP(kpp),
                () => CheckKPPUnique(kpp)
            });
        }

        /// <summary>
        /// Проверка КПП при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckKPPOnUpdate(AccountViewModel accountViewModel)
        {
            string kpp = accountViewModel.KPP?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckKPP(kpp),
                () => CheckKPPUnique(kpp, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка КПП на корректность
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPP(string kpp)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckKPPLength(kpp),
                () => CheckKPPSymbols(kpp),
                () => CheckKPPRight(kpp)
            });
        }

        /// <summary>
        /// Проверка КПП на длину
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPPLength(string kpp)
        {
            if (string.IsNullOrEmpty(kpp) || kpp.Length != KPP_LENGTH)
                errors.Add("KPPLength", resManager.GetString("KPPLength"));
        }

        /// <summary>
        /// Проверка КПП на недопустимые символы
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPPSymbols(string kpp)
        {
            if (ONLY_DIGITS.IsMatch(kpp))
                errors.Add("KPPWrong", resManager.GetString("KPPWrong"));
        }

        /// <summary>
        /// Проверка КПП по алгоритму
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPPRight(string kpp)
        {

        }

        /// <summary>
        /// Проверка, что не существует клиента с таким КПП (используется при создании)
        /// </summary>
        /// <param name="kpp"></param>
        private void CheckKPPUnique(string kpp)
        {
            Account accountWithSameKPP = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(k => k.KPP == kpp);
            if (accountWithSameKPP != null)
                errors.Add("KPPNotUnique", resManager.GetString("KPPNotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким КПП (используется при обновлении, так как необходимо исключить КПП обновляемого клиента из поиска)
        /// </summary>
        /// <param name="kpp"></param>
        /// <param name="updatedAccount"></param>
        private void CheckKPPUnique(string kpp, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameKPP = orgAccounts.FirstOrDefault(k => k.KPP == kpp && k.Id != updatedAccount.Id);
            if (accountWithSameKPP != null)
                errors.Add("KPPNotUnique", resManager.GetString("KPPNotUnique"));
        }

        /// <summary>
        /// Проверка ОКПО при создании клиента
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOOnCreate(AccountViewModel accountViewModel)
        {
            string okpo = accountViewModel.OKPO?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOKPO(okpo),
                () => CheckOKPOUnique(okpo)
            });
        }

        /// <summary>
        /// Проверка ОКПО при обновлении клиента
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOOnUpdate(AccountViewModel accountViewModel)
        {
            string okpo = accountViewModel.OKPO?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOKPO(okpo),
                () => CheckOKPOUnique(okpo, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ОКПО на корректность
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOKPO(string okpo)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOKPOLength(okpo),
                () => CheckOKPOSymbols(okpo),
                () => CheckOKPORight(okpo)
            });
        }

        /// <summary>
        /// Проверка ОКПО на длину
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOLength(string okpo)
        {
            if (string.IsNullOrEmpty(okpo) || okpo.Length != OKPO_LENGTH)
                errors.Add("OKPOLength", resManager.GetString("OKPOLength"));
        }

        /// <summary>
        /// Проверка ОКПО на недопустимые символы
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOSymbols(string okpo)
        {
            if (ONLY_DIGITS.IsMatch(okpo))
                errors.Add("OKPOWrong", resManager.GetString("OKPOWrong"));
        }

        /// <summary>
        /// Проверка ОКПО по алгоритму
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPORight(string okpo)
        {
            // Нахождение суммы произведений цифр ОКПО на соответствующий множитель
            int firstNumeralSumm = 0;
            for (int numeralIndex = 0; numeralIndex < OKPO_LENGTH - 1; numeralIndex++)
            {
                string numeral = okpo[numeralIndex].ToString();
                firstNumeralSumm += Convert.ToInt32(numeral) * OKPO_NUM_FACTORS_FIRST_CHECK[numeralIndex];
            }

            // Число "firstNumeralSumm" делится на константу OKPO_DIVIDER для получения остатка от деления
            int firstNumeralQuotient = firstNumeralSumm % OKPO_DIVIDER;

            // Получение контрольного числа
            int checkNumeral;

            // Если остаток от деления не равен 10, то контрольному числу присваивается это значение
            if (firstNumeralQuotient != 10)
                checkNumeral = firstNumeralQuotient;

            // Иначе необходим пересчет
            else
            {
                // Повторное нахождение суммы произведений цифр ОКПО на соответствующий множитель
                int lastNumeralSumm = 0;
                for (int numeralIndex = 0; numeralIndex < OKPO_LENGTH - 1; numeralIndex++)
                {
                    string numeral = okpo[numeralIndex].ToString();
                    lastNumeralSumm += Convert.ToInt32(numeral) * OKPO_NUM_FACTORS_LAST_CHECK[numeralIndex];
                }

                // Число "lastNumeralSumm" делится на константу OKPO_DIVIDER для получения остатка от деления
                int lastNumeralQuotient = lastNumeralSumm % OKPO_DIVIDER;

                // Если после повторной проверки с другими множителями значение остатка от деления остается равным 10, контрольное число становится равным 0, иначе - остаток от деления
                if (lastNumeralQuotient != 10)
                    checkNumeral = lastNumeralQuotient;
                else checkNumeral = 0;
            }

            // Проверка на равенство контрольного числа с 8-й цифрой ОКПО
            string lastOKPONumeral = okpo[OKPO_LENGTH - 1].ToString();
            if (checkNumeral != Convert.ToInt32(lastOKPONumeral))
                errors.Add("OKPOWrong", resManager.GetString("OKPOWrong"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОКПО (используется при создании)
        /// </summary>
        /// <param name="okpo"></param>
        private void CheckOKPOUnique(string okpo)
        {
            Account accountWithSameOKPO = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(o => o.OKPO == okpo);
            if (accountWithSameOKPO != null)
                errors.Add("OKPONotUnique", resManager.GetString("OKPONotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОКПО (используется при обновлении, так как необходимо исключить ОКПО обновляемого клиента из поиска)
        /// </summary>
        /// <param name="okpo"></param>
        /// <param name="updatedAccount"></param>
        private void CheckOKPOUnique(string okpo, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameOKPO = orgAccounts.FirstOrDefault(o => o.OKPO == okpo && o.Id != updatedAccount.Id);
            if (accountWithSameOKPO != null)
                errors.Add("OKPONotUnique", resManager.GetString("OKPONotUnique"));
        }

        /// <summary>
        /// Проверка ОГРН при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOGRNOnCreate(AccountViewModel accountViewModel)
        {
            string ogrn = accountViewModel.OGRN?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOGRN(ogrn),
                () => CheckOGRNUnique(ogrn)
            });
        }

        /// <summary>
        /// Проверка ОГРН при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckOGRNOnUpdate(AccountViewModel accountViewModel)
        {
            string ogrn = accountViewModel.OGRN?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOGRN(ogrn),
                () => CheckOGRNUnique(ogrn, accountViewModel)
            });
        }

        /// <summary>
        /// Проверка ОГРН на корректность
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRN(string ogrn)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckOGRNLength(ogrn),
                () => CheckOGRNSymbols(ogrn),
                () => CheckOGRNRight(ogrn)
            });
        }

        /// <summary>
        /// Проверка ОГРН на длину
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNLength(string ogrn)
        {
            if (string.IsNullOrEmpty(ogrn) || ogrn.Length != OGRN_LENGTH)
                errors.Add("OGRNLength", resManager.GetString("OGRNLength"));
        }

        /// <summary>
        /// Проверка ОГРН на недопустимые символы
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNSymbols(string ogrn)
        {
            if (ONLY_DIGITS.IsMatch(ogrn))
                errors.Add("OGRNWrong", resManager.GetString("OGRNWrong"));
        }

        /// <summary>
        /// Проверка ОГРН по алгоритму
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNRight(string ogrn)
        {
            // Вычисление остатка от деления первых 12 цифр ОГРН на константу "OGRN_DIVIDER"
            string ogrnNumerals = ogrn.TakeToString(OGRN_LENGTH - 1);
            string numeralQuotient = (Convert.ToInt64(ogrnNumerals) % OGRN_DIVIDER).ToString();

            // Получение контрольного числа - младшего разряда числа, полученного на предыдущем шаге
            int leastSignificantDigit = Convert.ToInt32(numeralQuotient[^1].ToString());

            // Проверка на равенство контрольного числа с 13-й цифрой ОГРН
            string lastOGRNNumeral = ogrn[OGRN_LENGTH - 1].ToString();
            if (leastSignificantDigit != Convert.ToInt32(lastOGRNNumeral))
                errors.Add("OGRNWrong", resManager.GetString("OGRNWrong"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОГРН (используется при создании)
        /// </summary>
        /// <param name="ogrn"></param>
        private void CheckOGRNUnique(string ogrn)
        {
            Account accountWithSameOGRN = context.GetAccountsByType(currentUser.PrimaryOrganizationId, AccountType.LegalEntity).FirstOrDefault(o => o.OGRN == ogrn);
            if (accountWithSameOGRN != null)
                errors.Add("OGRNNotUnique", resManager.GetString("OGRNNotUnique"));
        }

        /// <summary>
        /// Проверка, что не существует клиента с таким ОГРН (используется при обновлении, так как необходимо исключить ОГРН обновляемого клиента из поиска)
        /// </summary>
        /// <param name="ogrn"></param>
        /// <param name="updatedAccount"></param>
        private void CheckOGRNUnique(string ogrn, AccountViewModel updatedAccount)
        {
            // Список всех клиентов той организации, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(updatedAccount.OrganizationId);
            Account accountWithSameOGRN = orgAccounts.FirstOrDefault(o => o.OGRN == ogrn && o.Id != updatedAccount.Id);
            if (accountWithSameOGRN != null)
                errors.Add("OGRNNotUnique", resManager.GetString("OGRNNotUnique"));
        }

        /// <summary>
        /// Проверка страны на существование
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckCountry(AccountViewModel accountViewModel)
        {
            string country = accountViewModel.Country?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckCountryLength(country),
                () => CheckCountryExists(country)
            });
        }

        /// <summary>
        /// Метод проверяет название страны на длину
        /// </summary>
        /// <param name="country"></param>
        private void CheckCountryLength(string country)
        {
            if (string.IsNullOrEmpty(country))
                errors.Add("CoutnryLength", resManager.GetString("CoutnryLength"));
        }

        /// <summary>
        /// Проверка страны на существование
        /// </summary>
        /// <param name="country"></param>
        private void CheckCountryExists(string country)
        {
            JArray countries = AppUtils.GetCountries(currentUser?.DefaultLanguage);
            Func<JToken, bool> predicate = n => n.ToString().ToLower() == country.ToLower().TrimStartAndEnd();
            JToken findCountry = countries.FirstOrDefault(predicate);
            if (findCountry == null)
                errors.Add("CountryNotExists", resManager.GetString("CountryNotExists"));
        }

        /// <summary>
        /// Проверка выбранного менеджера
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckPrimaryManager(AccountViewModel accountViewModel)
        {
            string initialManagerName = accountViewModel.PrimaryManagerInitialName?.TrimStartAndEnd();
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckManagerNameLength(initialManagerName),
                () => CheckManagerExists(initialManagerName)
            });
        }

        /// <summary>
        /// Проверка имени менеджера на длину
        /// </summary>
        /// <param name="initialManagerName"></param>
        private void CheckManagerNameLength(string initialManagerName)
        {
            if (string.IsNullOrEmpty(initialManagerName))
                errors.Add("PrimaryManagerNameLength", resManager.GetString("PrimaryManagerNameLength"));
        }

        /// <summary>
        /// Проверка менеджера с таким именем на существование
        /// </summary>
        /// <param name="initialManagerName"></param>
        private void CheckManagerExists(string initialManagerName)
        {
            List<Employee> orgEmployees = context.GetOrgEmployees(currentUser.PrimaryOrganizationId);
            Func<Employee, bool> predicate = n => n.GetIntialsFullName().ToLower() == initialManagerName.ToLower();
            Employee manager = orgEmployees.FirstOrDefault(predicate);
            if (manager == null)
                errors.Add("ManagerNotExists", resManager.GetString("ManagerNotExists"));
        }

        /// <summary>
        /// Проверка названия ИП при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameOnCreate(AccountViewModel accountViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckIENameLength(accountViewModel),
                () => CheckIENameNotExistsOnCreate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия ИП при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameOnUpdate(AccountViewModel accountViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckIENameLength(accountViewModel),
                () => CheckIENameNotExistsOnUpdate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия ИП на длину
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameLength(AccountViewModel accountViewModel)
        {
            if (string.IsNullOrEmpty(accountViewModel.Name?.TrimStartAndEnd()))
                errors.Add("IENameLength", resManager.GetString("IENameLength"));
        }

        /// <summary>
        /// Проверка на отсутствие ИП с таким же именем в бд
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameNotExistsOnCreate(AccountViewModel accountViewModel)
        {
            List<Account> orgAccounts = context.GetOrgAccounts(currentUser.PrimaryOrganizationId);
            Account ieWithSameName = orgAccounts.FirstOrDefault(n => n.Name == accountViewModel.Name.TrimStartAndEnd());
            if (ieWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие ИП с таким же именем в бд (используется при обновлении, так как необходимо исключить обновляемого клиента из поиска)
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckIENameNotExistsOnUpdate(AccountViewModel accountViewModel)
        {
            // Список всех клиентов, созданных организацией, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(accountViewModel.OrganizationId);
            Account ieWithSameName = orgAccounts.FirstOrDefault(acc => acc.Name == accountViewModel.Name.TrimStartAndEnd() && acc.Id != accountViewModel.Id);
            if (ieWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка названия юридического лица при создании клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameOnCreate(AccountViewModel accountViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckLENameLength(accountViewModel),
                () => CheckLENameNotExistsOnCreate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия юридического лица при обновлении клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameOnUpdate(AccountViewModel accountViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckLENameLength(accountViewModel),
                () => CheckLENameNotExistsOnUpdate(accountViewModel)
            });
        }

        /// <summary>
        /// Проверка названия юридического лица на длину
        /// </summary>
        /// <param name="leName">Название юридического лица</param>
        private void CheckLENameLength(AccountViewModel accountViewModel)
        {
            if (string.IsNullOrEmpty(accountViewModel.Name?.TrimStartAndEnd()))
                errors.Add("LENameLength", resManager.GetString("LENameLength"));
        }

        /// <summary>
        /// Проверка на отсутствие юридического лица с таким же именем в бд
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameNotExistsOnCreate(AccountViewModel accountViewModel)
        {
            List<Account> orgAccounts = context.GetOrgAccounts(currentUser.PrimaryOrganizationId);
            Account leWithSameName = orgAccounts.FirstOrDefault(n => n.Name == accountViewModel.Name.TrimStartAndEnd());
            if (leWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие юридического лица с таким же именем в бд (используется при обновлении, так как необходимо исключить обновляемого клиента из поиска)
        /// </summary>
        /// <param name="accountViewModel"></param>
        private void CheckLENameNotExistsOnUpdate(AccountViewModel accountViewModel)
        {
            // Список всех клиентов, созданных организацией, к которой относится обновляемый клиент
            List<Account> orgAccounts = context.GetOrgAccounts(accountViewModel.OrganizationId);
            Account leWithSameName = orgAccounts.FirstOrDefault(acc => acc.Name == accountViewModel.Name.TrimStartAndEnd() && acc.Id != accountViewModel.Id);
            if (leWithSameName != null)
                errors.Add("AccountAlreadyExists", resManager.GetString("AccountAlreadyExists"));
        }

        /// <summary>
        /// Проверка, что при обновлении клиента не изменился его тип
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="account">Изменяемый клиент</param>
        private void CheckAccountTypeNotChanged(AccountViewModel accountViewModel, Account account)
        {
            AccountType accountType = (AccountType)Enum.Parse(typeof(AccountType), accountViewModel.AccountType);
            if (accountType != account.AccountType)
                errors.Add("AccountTypeIsReadonly", resManager.GetString("AccountTypeIsReadonly"));
        }

        /// <summary>
        /// Проверка, что у клиента присутствует основной клиентский менеджер
        /// </summary>
        /// <param name="account"></param>
        private void CheckManagerExists(Account account)
        {
            if (context.AccountManagers.FirstOrDefault(i => i.Id == account.PrimaryManagerId) == null)
                errors.Add("AccountWithoutKMIsReadonly", resManager.GetString("AccountWithoutKMIsReadonly"));
        }

        /// <summary>
        /// Проверка, что при обновлении физического лица не менялось полное имя клиента
        /// </summary>
        /// <param name="accountViewModel"></param>
        /// <param name="account">Изменяемый клиент</param>
        private Dictionary<string, string> CheckIndividualNameNotChanged(AccountViewModel accountViewModel, Account account)
        {
            string fullName = accountViewModel.Name?.TrimStartAndEnd();
            if (account.Name != fullName)
                errors.Add("IndividualNameIsReadonly", resManager.GetString("IndividualNameIsReadonly"));
            return errors;
        }
    }
}
