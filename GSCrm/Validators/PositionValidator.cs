using GSCrm.Data;
using GSCrm.Helpers;
using GSCrm.Localization;
using GSCrm.Models;
using GSCrm.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static GSCrm.Utils.CollectionsUtils;

namespace GSCrm.Validators
{
    public class PositionValidator : BaseValidator<PositionViewModel>
    {
        private const int POSITION_NAME_MIN_LENGTH = 3;
        public PositionValidator(ApplicationDbContext context, ResManager resManager) : base(context, resManager) { }

        /// <summary>
        /// Проверка должности при создании
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <returns></returns>
        public override Dictionary<string, string> CreationCheck(PositionViewModel positionViewModel)
        {
            Division division = null;
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckPositionLength(positionViewModel),
                () => CheckDivisionLength(positionViewModel),
                () =>
                {
                    if (TryGetDivision(positionViewModel, out division))
                        CheckPositionNotExists(positionViewModel, division);
                },
                () =>
                {
                    if (!string.IsNullOrEmpty(positionViewModel.ParentPositionName))
                        TryGetParentPosition(positionViewModel, division, out Position _);
                },
                () =>
                {
                    if (!string.IsNullOrEmpty(positionViewModel.PrimaryEmployeeInitialName))
                        CheckPrimaryEmployeeExists(positionViewModel);
                }
            });
            return errors;
        }

        /// <summary>
        /// Проверка должности при обновлении
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public override Dictionary<string, string> UpdateCheck(PositionViewModel positionViewModel)
        {
            Division division = null;
            Position parentPosition = null;
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckPositionLength(positionViewModel),
                () => CheckDivisionLength(positionViewModel),
                () =>
                {
                    if (TryGetDivision(positionViewModel, out division))
                        CheckPositionNotExistsOnUpdate(positionViewModel, division);
                },
                () =>
                {
                    if (!string.IsNullOrEmpty(positionViewModel.ParentPositionName))
                    {
                        InvokeIntermittingChecks(errors, new List<Action>()
                        {
                            () => CheckPositionsNotCompare(positionViewModel),
                            // При обновлении вначале требуется проверить родительскую должность на существование
                            () => TryGetParentPosition(positionViewModel, division, out parentPosition),
                            // Затем требуется убедиться, что при установке родительской должности не произойдет циклической зависимости,
                            // когда для Position 2 родительской является Position 1, и для Position 1 устанавливается в качесте родительской Position 2
                            () => CheckPositionsHierarchy(context.Positions.FirstOrDefault(i => i.Id == positionViewModel.Id), positionViewModel.Name, parentPosition)
                        });
                    }
                },
                () =>
                {
                    if (!string.IsNullOrEmpty(positionViewModel.PrimaryEmployeeInitialName))
                        CheckPrimaryEmployeeExists(positionViewModel);
                }
            });
            return errors;
        }

        /// <summary>
        /// Проверка должности при смене подразделения
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <returns></returns>
        public Dictionary<string, string> ChangeDivisionCheck(PositionViewModel positionViewModel)
        {
            InvokeIntermittingChecks(errors, new List<Action>()
            {
                () => CheckDivisionLength(positionViewModel),
                () =>
                {
                    if (TryGetDivision(positionViewModel, out Division newDivision))
                        CheckDivisionsNotCompare(positionViewModel, newDivision);
                }
            });
            return errors;
        }

        /// <summary>
        /// Проверка длины названия должности
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionLength(PositionViewModel positionViewModel)
        {
            positionViewModel.Name = positionViewModel.Name.TrimStartAndEnd();
            if (string.IsNullOrEmpty(positionViewModel.Name) || positionViewModel.Name.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("PositionNameLength", resManager.GetString("PositionNameLength"));
        }

        /// <summary>
        /// Проверка длины названия подразделения
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckDivisionLength(PositionViewModel positionViewModel)
        {
            positionViewModel.DivisionName = positionViewModel.DivisionName.TrimStartAndEnd();
            if (string.IsNullOrEmpty(positionViewModel.DivisionName) || positionViewModel.DivisionName.Length < POSITION_NAME_MIN_LENGTH)
                errors.Add("DivisionNameLength", resManager.GetString("DivisionNameLength"));
        }

        /// <summary>
        /// Метод проверяет, что существует подразделение с таким названием и возвращает его
        /// </summary>
        /// <param name="positionViewModel"></param>
        private bool TryGetDivision(PositionViewModel positionViewModel, out Division division)
        {
            Organization organization = positionViewModel.GetOrganization(context);
            division = organization.Divisions.FirstOrDefault(n => n.Name == positionViewModel.DivisionName);
            if (division == null)
            {
                errors.Add("DivisionNotExists", resManager.GetString("DivisionNotExists"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Метод проверяет, что выбрано отличное от текущего подразделение
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckDivisionsNotCompare(PositionViewModel positionViewModel, Division newDivision)
        {
            Position position = context.Positions.Include(div => div.Division).FirstOrDefault(i => i.Id == positionViewModel.Id);
            if (position.Division.Name == newDivision.Name)
                errors.Add("ThisPositionDivisionIsAlreadySelect", resManager.GetString("ThisPositionDivisionIsAlreadySelect"));
        }

        /// <summary>
        /// Проверка на отсутствие должности с таким же названием в этом подразделении
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionNotExists(PositionViewModel positionViewModel, Division division)
        {
            Position positionWithSameName = division.Positions.FirstOrDefault(n => n.Name == positionViewModel.Name);
            if (!string.IsNullOrEmpty(positionWithSameName?.Name))
                errors.Add("PositionAlreadyExists", resManager.GetString("PositionAlreadyExists"));
        }

        /// <summary>
        /// Проверка на отсутствие должности с таким же названием в этом подразделении при обновлении записи
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPositionNotExistsOnUpdate(PositionViewModel positionViewModel, Division division)
        {
            Position positionWithSameName = division.Positions.FirstOrDefault(n => n.Name == positionViewModel.Name);
            if (positionWithSameName != null && positionWithSameName.Id != positionViewModel.Id)
                errors.Add("PositionAlreadyExists", resManager.GetString("PositionAlreadyExists"));
        }

        /// <summary>
        /// Метод пытается получить родительскую должность
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="division"></param>
        /// <param name="parentPosition"></param>
        private bool TryGetParentPosition(PositionViewModel positionViewModel, Division division, out Position parentPosition)
        {
            parentPosition = positionViewModel.GetParentPosition(division);
            if (parentPosition == null)
            {
                errors.Add("PositionNotExists", resManager.GetString("PositionNotExists"));
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Метод проверряет, что устанавливаемая как родительская должность отсутсвует в списке дочерних должностей у обновляемой
        /// </summary>
        /// <param name="currentPosition">Текущая изменяемая должность</param>
        /// <param name="currentPositinoName">Название текущей изменяемой должность, так как оно может измениться в процессе обновления</param>
        /// <param name="newParentPosition">Новая родительская должность</param>
        private void CheckPositionsHierarchy(Position currentPosition, string currentPositinoName, Position newParentPosition)
        {
            if (currentPosition.IsParentPositionFor(newParentPosition, context))
                errors.Add("PositionCannotBeParent", new StringBuilder()
                    .Append("Невозможно добавить должнось ").Append(newParentPosition.Name)
                    .Append(" как родительскую для ").Append(currentPositinoName)
                    .Append(". Так как добавляемая должность находится в дочерних у текущей.")
                    .ToString());
        }

        /// <summary>
        /// Проверка, что навзвание текущей должности не совпадает с названием родительской
        /// </summary>
        /// <param name="positionViewModel"></param>
        /// <param name="parentPosition"></param>
        private void CheckPositionsNotCompare(PositionViewModel positionViewModel)
        {
            if (positionViewModel.Name == positionViewModel.ParentPositionName)
                errors.Add("ParentAndCurrentPositionsCompare", resManager.GetString("ParentAndCurrentPositionsCompare"));
        }

        /// <summary>
        /// Проверка на существование сотрудника с полученным id
        /// </summary>
        /// <param name="positionViewModel"></param>
        private void CheckPrimaryEmployeeExists(PositionViewModel positionViewModel)
        {
            if (positionViewModel.PrimaryEmployeeId == null)
            {
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
                return;
            }
            if (positionViewModel.GetPrimaryEmployee(context) == null)
                errors.Add("EmployeeNotExists", resManager.GetString("EmployeeNotExists"));
        }
    }
}
