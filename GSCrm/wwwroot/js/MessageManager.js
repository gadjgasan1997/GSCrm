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

            /** ��������� � �������������� �������� ���� ���������� ����������� ��� ������������� */
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

            /** ��������� � �������������� ����� ������������� */
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

            /** ������ �������� ���������� */
            RemoveEmployeeError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeEmpError")
                    }
                }
            },

            /** ������ �������� �������� ���������� */
            RemoveEmployeeContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeEmpContactError")
                    }  
                }
            },

            /** ��������� � �������������� �������� ����������� */
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

            /** ��������� � �������������� ������ �� ����������� */
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

            /** ��������� � �������������� �������� ��������� */
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

            /** ������ �������� ��������� */
            RemovePositionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removePosError")
                    }
                }
            },

            /** ��������� � �������������� ��������� ������������� */
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

            /** ��������� � �������������� �������� ������������� */
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

            /** ������ �������� ������������� */
            RemoveDivisionError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeDivError")
                    }
                }
            },

            /** ��������� � ������������ ������ �������� ������ ����� */
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

            /** ��������� �� �������� ��������� ����� */
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

            /** ��������� � ���������� ��������� ����� */
            SiteHasNotBeenChanged: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("siteHasNotBeenChanged"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** ������ �������� �������� ������� */
            RemoveAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("removeAccContactError"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** ������ ����� ��������� �������� ������� */
            ChangePrimaryAccountContactError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("changeAccPrimaryContactError"),
                        html: inputProperties["error"]
                    }
                }
            },

            /** ��������� �� �������� ��������� ��������� �������� �� ������� */
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

            /** ��������� �� �������� ��������� �������� ����������� */
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

            /** ��������� � �����������, ����������� ��� ����� ���� �� ����������� ������ */
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

            /** ��������� � ���, ��� � ������� ��� ��������� ������� */
            AddressListIsEmpty: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("addressListIsEmpty"),
                        text: Localization.GetString("addressListIsEmptyForChangeLegal")
                    }
                }
            },

            /** ��������� �� �������� ��������� ������������ ������ */
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

            /** ��������� � �������������� �������� ������� */
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

            /** ������ �������� ������� */
            RemoveAccountError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("errorLabel"),
                        text: Localization.GetString("removeAccError")
                    }
                }
            },

            /** ��������� � �������������� �������� ���������� */
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

            /** ��������� � ������������ ��������� �������� ���������� */
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

            /** ��������� � �������������� �������� ���� ��� ������� */
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

            /** ������ ������������� ���������� */
            SyncPositionsError: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("syncPositionsError"),
                        text: inputProperties["error"]
                    }
                }
            },

            /** ������������ ���������� */
            HasNotPermissions: class {
                Initialize(inputProperties) {
                    return {
                        icon: 'error',
                        title: Localization.GetString("hasNotPermissions"),
                        text: inputProperties["error"]
                    }
                }
            },

            /** ���������, ��� ��������� ����������� ���� ������� �������� */
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

            /** ���������, ��� ��������� ����������� ���� ������� ����������� �� ��������� */
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

            /** ��������� � �������������� ��������� �������� ����������� � ������������ �� ��������� */
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

            /** ��������� � �������������� ��������� �������� ������ ����������� �� ��������� */
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

            /** ��������� � �����������, ��� ����������� ���� ��������� */
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