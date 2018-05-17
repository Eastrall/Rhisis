using Catel.Data;
using Catel.MVVM;
using Orc.Wizard;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.ServerManager.Wizards.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Rhisis.ServerManager.Wizards.ViewModels
{

    public class DatabaseConfigurationPageViewModel : WizardPageViewModelBase<DatabaseConfigurationPage>
    {
        public DatabaseConfigurationPageViewModel(DatabaseConfigurationPage wizardPage) : base(wizardPage)
        {
            this.Providers = Enum.GetValues(typeof(DatabaseProvider)).Cast<DatabaseProvider>();
            this.TestDatabaseConnectionCommand = new Command(OnTestDatabaseConnectionCommand);
        }

        #region Properties

        [ViewModelToModel]
        public string Host { get; set; }

        [ViewModelToModel]
        public int Port { get; set; }

        [ViewModelToModel]
        public string Username { get; set; }

        [ViewModelToModel]
        public string Password { get; set; }

        [ViewModelToModel]
        public string Database { get; set; }

        [ViewModelToModel]
        public DatabaseProvider Provider { get; set; }

        public IEnumerable<DatabaseProvider> Providers { get; }

        public string Message { get; set; }

        public bool IsTestSuccess { get; set; }

        #endregion

        #region

        public Command TestDatabaseConnectionCommand { get; }

        private void OnTestDatabaseConnectionCommand()
        {
            DatabaseContext context = null;

            try
            {
                DatabaseService.Configure(Host, Port, Username, Password, Database, Provider);
                context = DatabaseService.GetContext();
                context.OpenConnection();
                Message = "Successfully connected to the database";
                IsTestSuccess = true;
            }
            catch (DbException e)
            {
                Message = e.Message;
                IsTestSuccess = false;
            }
            catch (Exception e)
            {
                //TODO: try to remove
                Message = e.Message;
                IsTestSuccess = false;
            }
            finally
            {
                context?.Dispose();
                DatabaseService.UnloadConfiguration();
            }
        }

        #endregion

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            base.ValidateBusinessRules(validationResults);
            if (!IsTestSuccess)
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("The database is not corretly configured"));
            }
        }
    }
}
