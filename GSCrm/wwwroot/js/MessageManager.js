class MessageManager {
    static Invoke(messageName, inputProperties) {
        let messages = new {
            CommonError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        html:  inputProperties["error"]
                    }
                }
            },

            /** Сообщение с подтверждением закрытия окна управления должностями без синхронизации */
            PositionModalClosedConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("attentionModalClosing"),
                        text: LocalizationManager.GetString("positionModalClosingConfirm"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptModalClosing"),
                        cancelButtonText: LocalizationManager.GetString("declineModalClosing")
                    }
                }
            },

            /** Сообщение с подтверждением смены подразделения */
            ChangeEmpDivisionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("divisionChanging"),
                        text: LocalizationManager.GetString("employeeDivChangingConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptItemChanging"),
                        cancelButtonText: LocalizationManager.GetString("declineItemChanging"),
                    }
                }
            },

            /** Ошибка удаления сотрудника */
            RemoveEmployeeError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("removeEmpError")
                    }
                }
            },

            /** Ошибка удаления контакта сотрудника */
            RemoveEmployeeContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("removeEmpContactError")
                    }  
                }
            },

            /** Сообщение с подтверждением удаления организации */
            RemoveOrgConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("orgRemoving"),
                        text: LocalizationManager.GetString("removeOrgConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptItemRemove"),
                        cancelButtonText: LocalizationManager.GetString("declineItemRemove"),
                    }
                }
            },

            /** Сообщение с подтверждением выхода из организации */
            LeaveOrgConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("orgLeaving"),
                        text: LocalizationManager.GetString("leaveOrgConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("declineLeave"),
                        cancelButtonText: LocalizationManager.GetString("cancel"),
                    }
                }
            },

            /** Сообщение с подтверждением удаления должности */
            RemovePositionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("positionRemoving"),
                        text: LocalizationManager.GetString("removePositionConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptRemove"),
                        cancelButtonText: LocalizationManager.GetString("decline"),
                    }
                }
            },

            /** Ошибка удаления должности */
            RemovePositionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("removePosError")
                    }
                }
            },

            /** Сообщение с подтверждением изменения подразделения */
            ChangePosDivisionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("divisionChanging"),
                        text: LocalizationManager.GetString("positionDivChangingConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptItemChanging"),
                        cancelButtonText: LocalizationManager.GetString("declineItemChanging"),
                    }
                }
            },

            /** Сообщение с подтверждением удаления подразделения */
            RemoveDivConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("divisionRemoving"),
                        text: LocalizationManager.GetString("removeDivisionConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptItemRemove"),
                        cancelButtonText: LocalizationManager.GetString("declineItemRemove"),
                    }  
                }
            },

            /** Ошибка удаления подразделения */
            RemoveDivisionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("removeDivError")
                    }
                }
            },

            /** Сообщение с предложением ввести название нового сайта */
            NewSiteMessage: class {
                Initialize(inputProperties) {
                    let siteInput = inputProperties["siteInput"];
                    let changeSiteUrl = inputProperties["changeSiteUrl"];
                    let sitePlaceholder = siteInput.attr("href");
                    if (sitePlaceholder == undefined) {
                        sitePlaceholder = LocalizationManager.GetString("siteName");
                    }
                    return {
                        title: LocalizationManager.GetString("siteChanging"),
                        input: 'text',
                        inputAttributes: {
                        autocapitalize: 'off'
                        },
                        showCancelButton: true,
                        confirmButtonText: LocalizationManager.GetString("change"),
                        showLoaderOnConfirm: true,
                        cancelButtonText: LocalizationManager.GetString("cancel"),
                        inputValue: sitePlaceholder,
                        preConfirm: (newSite) => {
                            let request = new AjaxRequests();
                            let newSiteUrl = changeSiteUrl + newSite;
                            return request.CommonGetRequest(newSiteUrl)
                                .fail(response => {
                                    Utils.CommonErrosHandling(response["responseJSON"], ["ChangeSite"]);
                                })
                                .done(() => {
                                    Swal.fire(MessageManager.Invoke("SiteHasBeenChanged"));
                                    $(siteInput).text(newSite);
                                    $(siteInput).attr("href", newSite);
                                });
                        },
                        allowOutsideClick: () => !Swal.isLoading()
                    }
                }
            },

            /** Сообщение об успешном изменении сайта */
            SiteHasBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("siteHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение о неуспешном изменении сайта */
            SiteHasNotBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("siteHasNotBeenChanged"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** Ошибка удаления контакта клиента */
            RemoveAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("removeAccContactError"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** Ошибка смены основного контакта клиента */
            ChangePrimaryAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("changeAccPrimaryContactError"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** Сообщение об успешном изменении основного контакта на клиенте */
            PrimaryContactHasBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("primaryContactHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение об успешном изменении основной организации */
            PrimaryOrgHasBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("primaryOrgHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с информацией, возникающее при смене типа на юридическом адресе */
            ChangeLegalAddressInfo: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("changingAddressType"),
                        text: LocalizationManager.GetString("changingAddressTypeInfo"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("selectAddress"),
                        cancelButtonText: LocalizationManager.GetString("declineModalClosing")
                    }
                }
            },

            /** Сообщение о том, что у клиента нет свободных адресов */
            AddressListIsEmpty: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("addressListIsEmpty"),
                        text: LocalizationManager.GetString("addressListIsEmptyForChangeLegal")
                    }
                }
            },

            /** Сообщение об успешном изменении юридического адреса */
            LegalAddressHasBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("legalAddressHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с подтверждением удаления клиента */
            RemoveAccountConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("accountRemoving"),
                        text: LocalizationManager.GetString("removeAccConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptRemove"),
                        cancelButtonText: LocalizationManager.GetString("decline"),
                    }
                }
            },

            /** Ошибка удаления клиента */
            RemoveAccountError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("removeAccError")
                    }
                }
            },

            /** Сообщение с подтверждением удаления сотрудника */
            RemoveEmployeeConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("employeeRemoving"),
                        text: LocalizationManager.GetString("removeEmployeeConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptRemove"),
                        cancelButtonText: LocalizationManager.GetString("decline"),
                    }
                }
            },

            /** Сообщение с предложением измениния названия полномочия */
            ChangingReponsibilityName: class {
                Initialize(inputProperties) {
                    let currentReponsibilityName = inputProperties["currentReponsibilityName"];
                    return {
                        title: LocalizationManager.GetString("changingReponsibilityName"),
                        input: 'text',
                        inputAttributes: {
                          autocapitalize: 'off'
                        },
                        showCancelButton: true,
                        confirmButtonText: LocalizationManager.GetString("change"),
                        cancelButtonText: LocalizationManager.GetString("declineModalClosing"),
                        showLoaderOnConfirm: true,
                        inputValue: currentReponsibilityName,
                        allowOutsideClick: () => !Swal.isLoading()
                    }
                }
            },

            /** Сообщение с подтверждением закрытия окна без коммита */
            NotCommitModalClosedConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("attentionModalClosing"),
                        text: LocalizationManager.GetString("notCommitModalClosingConfirm"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptModalClosing"),
                        cancelButtonText: LocalizationManager.GetString("declineModalClosing")
                    }
                }
            },

            /** Ошибка синхронизации должностей */
            SyncPositionsError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("syncPositionsError"),
                        text: inputProperties["error"]
                    }
                }
            },

            /** Недостаточно полномочий */
            HasNotPermissions: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("hasNotPermissions"),
                        text: inputProperties["error"]
                    }
                }
            },

            /** Сообщение, что настройки уведомлений были успешно изменены */
            NotSettingsCommitSuccess: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("notSettingsCommitSuccess"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение, что настройки уведомлений были успешно установлены по умолчанию */
            SetNotSettingsToDefaultSuccess: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("setNotSettingsToDefaultSuccess"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с подтверждением установки настроек уведомлений в организациях по умолчанию */
            ResetOrgNotSettingsConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("setNotSettingsToDefault"),
                        text: LocalizationManager.GetString("setOrgNotSettingsToDefaultConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptSetToDefault"),
                        cancelButtonText: LocalizationManager.GetString("decline"),
                    }
                }
            },

            /** Сообщение с подтверждением установки настроек личных уведомлений по умолчанию */
            ResetUserNotSettingsConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("setNotSettingsToDefault"),
                        text: LocalizationManager.GetString("setUserNotSettingsToDefaultConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptSetToDefault"),
                        cancelButtonText: LocalizationManager.GetString("decline"),
                    }
                }
            },

            /** Сообщение с информацией, что приглашение было откланено */
            InviteHasBeenRejected : class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("inviteHasBeenRejected"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с информацией, что приглашение было принято */
            InviteHasBeenAccepted : class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: LocalizationManager.GetString("inviteHasBeenAccepted"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с подтверждением удаления категории продуктов */
            RemoveCategoryConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: LocalizationManager.GetString("removeCategory"),
                        text: LocalizationManager.GetString("removeCategoryConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: LocalizationManager.GetString("acceptItemRemove"),
                        cancelButtonText: LocalizationManager.GetString("declineItemRemove")
                    }
                }
            },

            /** Недостаточно полномочий */
            ProductCategoryInitializeError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: LocalizationManager.GetString("errorLabel"),
                        text: LocalizationManager.GetString("productCategoryInitializeError")
                    }
                }
            }
        }[messageName];
        return messages.Initialize(inputProperties);
    }
}