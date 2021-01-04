class MessageManager {
    static Invoke(messageName, inputProperties) {
        let messages = new {
            CommonError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        html:  inputProperties["error"]
                    }
                }
            },

            /** Сообщение с подтверждением закрытия окна управления должностями без синхронизации */
            PositionModalClosedConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("attentionModalClosing"),
                        text: Localization.GetString("positionModalClosingConfirm"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptModalClosing"),
                        cancelButtonText: Localization.GetString("declineModalClosing")
                    }
                }
            },

            /** Сообщение с подтверждением смены подразделения */
            ChangeEmpDivisionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("divisionChanging"),
                        text: Localization.GetString("employeeDivChangingConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptItemChanging"),
                        cancelButtonText: Localization.GetString("declineItemChanging"),
                    }
                }
            },

            /** Ошибка удаления сотрудника */
            RemoveEmployeeError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeEmpError")
                    }
                }
            },

            /** Ошибка удаления контакта сотрудника */
            RemoveEmployeeContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeEmpContactError")
                    }  
                }
            },

            /** Сообщение с подтверждением удаления организации */
            RemoveOrgConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("orgRemoving"),
                        text: Localization.GetString("removeOrgConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptItemRemove"),
                        cancelButtonText: Localization.GetString("declineItemRemove"),
                    }
                }
            },

            /** Сообщение с подтверждением выхода из организации */
            LeaveOrgConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("orgLeaving"),
                        text: Localization.GetString("leaveOrgConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("declineLeave"),
                        cancelButtonText: Localization.GetString("cancel"),
                    }
                }
            },

            /** Сообщение с подтверждением удаления должности */
            RemovePositionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("positionRemoving"),
                        text: Localization.GetString("removePositionConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptRemove"),
                        cancelButtonText: Localization.GetString("decline"),
                    }
                }
            },

            /** Ошибка удаления должности */
            RemovePositionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removePosError")
                    }
                }
            },

            /** Сообщение с подтверждением изменения подразделения */
            ChangePosDivisionConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("divisionChanging"),
                        text: Localization.GetString("positionDivChangingConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptItemChanging"),
                        cancelButtonText: Localization.GetString("declineItemChanging"),
                    }
                }
            },

            /** Сообщение с подтверждением удаления подразделения */
            RemoveDivConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("divisionRemoving"),
                        text: Localization.GetString("removeDivisionConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptItemRemove"),
                        cancelButtonText: Localization.GetString("declineItemRemove"),
                    }  
                }
            },

            /** Ошибка удаления подразделения */
            RemoveDivisionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeDivError")
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
                        sitePlaceholder = Localization.GetString("siteName");
                    }
                    return {
                        title: Localization.GetString("siteChanging"),
                        input: 'text',
                        inputAttributes: {
                        autocapitalize: 'off'
                        },
                        showCancelButton: true,
                        confirmButtonText: Localization.GetString("change"),
                        showLoaderOnConfirm: true,
                        cancelButtonText: Localization.GetString("cancel"),
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
                        title: Localization.GetString("siteHasBeenChanged"),
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
                        title: Localization.GetString("siteHasNotBeenChanged"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** Ошибка удаления контакта клиента */
            RemoveAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("removeAccContactError"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** Ошибка смены основного контакта клиента */
            ChangePrimaryAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("changeAccPrimaryContactError"),
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
                        title: Localization.GetString("primaryContactHasBeenChanged"),
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
                        title: Localization.GetString("primaryOrgHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с информацией, возникающее при смене типа на юридическом адресе */
            ChangeLegalAddressInfo: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("changingAddressType"),
                        text: Localization.GetString("changingAddressTypeInfo"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("selectAddress"),
                        cancelButtonText: Localization.GetString("declineModalClosing")
                    }
                }
            },

            /** Сообщение о том, что у клиента нет свободных адресов */
            AddressListIsEmpty: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("addressListIsEmpty"),
                        text: Localization.GetString("addressListIsEmptyForChangeLegal")
                    }
                }
            },

            /** Сообщение об успешном изменении юридического адреса */
            LegalAddressHasBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: Localization.GetString("legalAddressHasBeenChanged"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с подтверждением удаления клиента */
            RemoveAccountConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("accountRemoving"),
                        text: Localization.GetString("removeAccConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptRemove"),
                        cancelButtonText: Localization.GetString("decline"),
                    }
                }
            },

            /** Ошибка удаления клиента */
            RemoveAccountError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeAccError")
                    }
                }
            },

            /** Сообщение с подтверждением удаления сотрудника */
            RemoveEmployeeConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("employeeRemoving"),
                        text: Localization.GetString("removeEmployeeConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptRemove"),
                        cancelButtonText: Localization.GetString("decline"),
                    }
                }
            },

            /** Сообщение с предложением измениния названия полномочия */
            ChangingReponsibilityName: class {
                Initialize(inputProperties) {
                    let currentReponsibilityName = inputProperties["currentReponsibilityName"];
                    return {
                        title: Localization.GetString("changingReponsibilityName"),
                        input: 'text',
                        inputAttributes: {
                          autocapitalize: 'off'
                        },
                        showCancelButton: true,
                        confirmButtonText: Localization.GetString("change"),
                        cancelButtonText: Localization.GetString("declineModalClosing"),
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
                        title: Localization.GetString("attentionModalClosing"),
                        text: Localization.GetString("notCommitModalClosingConfirm"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptModalClosing"),
                        cancelButtonText: Localization.GetString("declineModalClosing")
                    }
                }
            },

            /** Ошибка синхронизации должностей */
            SyncPositionsError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("syncPositionsError"),
                        text: inputProperties["error"]
                    }
                }
            },

            /** Недостаточно полномочий */
            HasNotPermissions: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("hasNotPermissions"),
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
                        title: Localization.GetString("notSettingsCommitSuccess"),
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
                        title: Localization.GetString("setNotSettingsToDefaultSuccess"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            },

            /** Сообщение с подтверждением установки настроек уведомлений в организациях по умолчанию */
            ResetOrgNotSettingsConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("setNotSettingsToDefault"),
                        text: Localization.GetString("setOrgNotSettingsToDefaultConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptSetToDefault"),
                        cancelButtonText: Localization.GetString("decline"),
                    }
                }
            },

            /** Сообщение с подтверждением установки настроек личных уведомлений по умолчанию */
            ResetUserNotSettingsConfirmation: class {
                Initialize(inputProperties) {
                    return {
                        title: Localization.GetString("setNotSettingsToDefault"),
                        text: Localization.GetString("setUserNotSettingsToDefaultConfirmation"),
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: Localization.GetString("acceptSetToDefault"),
                        cancelButtonText: Localization.GetString("decline"),
                    }
                }
            },

            /** Сообщение с информацией, что приглашение было откланено */
            InviteHasBeenRejected : class {
                Initialize(inputProperties) {
                    return {
                        position: 'top-end',
                        icon: 'success',
                        title: Localization.GetString("inviteHasBeenRejected"),
                        showConfirmButton: false,
                        timer: 1500
                    }
                }
            }
        }[messageName];
        return messages.Initialize(inputProperties);
    }
}