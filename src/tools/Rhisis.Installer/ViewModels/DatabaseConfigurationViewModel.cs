using Rhisis.Database;
using Rhisis.Tools.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Rhisis.Installer.Models;

namespace Rhisis.Installer.ViewModels
{
    public class DatabaseConfigurationViewModel : ViewModelBase
    {
        private readonly DatabaseConfiguration _databaseConfigurationCopy;
        private readonly AccountModel _account;

        /// <summary>
        /// Gets the Ok command.
        /// </summary>
        public ICommand OkCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Gets the test command.
        /// </summary>
        public ICommand TestCommand { get; }

        /// <summary>
        /// Gets the create account command.
        /// </summary>
        public ICommand CreateAccountCommand { get; }

        /// <summary>
        /// Gets the database configuration.
        /// </summary>
        public DatabaseConfiguration Configuration { get; }

        /// <summary>
        /// Gets list of providers.
        /// </summary>
        public IEnumerable<DatabaseProvider> Providers { get; }

        /// <summary>
        /// Creates a new <see cref="DatabaseConfigurationViewModel"/> instance.
        /// </summary>
        public DatabaseConfigurationViewModel()
            : this(new DatabaseConfiguration(), new AccountModel())
        {
        }

        /// <summary>
        /// Creates a new <see cref="DatabaseConfigurationViewModel"/> instance.
        /// </summary>
        public DatabaseConfigurationViewModel(DatabaseConfiguration configuration, AccountModel account)
        {
            this.OkCommand = new Command(this.OnOk);
            this.CancelCommand = new Command(this.OnCancel);
            this.TestCommand = new Command(this.OnTest);
            this.CreateAccountCommand = new Command(this.OnCreateAccount);
            this.Providers = Enum.GetValues(typeof(DatabaseProvider)).Cast<DatabaseProvider>();
            this.Configuration = configuration;
            this._databaseConfigurationCopy = new DatabaseConfiguration
            {
                Database = this.Configuration.Database,
                Host = this.Configuration.Host,
                Password = this.Configuration.Password,
                Port = this.Configuration.Port,
                Provider = this.Configuration.Provider,
                Username = this.Configuration.Username
            };
            this._account = account;
        }

        protected void OnOk() => this.Close();

        protected void OnCancel()
        {
            this.Configuration.Database = this._databaseConfigurationCopy.Database;
            this.Configuration.Host = this._databaseConfigurationCopy.Host;
            this.Configuration.Password = this._databaseConfigurationCopy.Password;
            this.Configuration.Port = this._databaseConfigurationCopy.Port;
            this.Configuration.Provider = this._databaseConfigurationCopy.Provider;
            this.Configuration.Username = this._databaseConfigurationCopy.Username;
            this.Close();
        }

        protected void OnTest()
        {
            DatabaseContext context = null;

            try
            {
                DatabaseService.Configure(this.Configuration);

                context = DatabaseService.GetContext();
                context.OpenConnection();

                this.Configuration.IsValid = true;
                this.DialogService.ShowInformation("Connection success", "The connection has succeeded.");
            }
            catch (DbException e)
            {
                this.DialogService.ShowError("Connection error", e.Message);
            }
            finally
            {
                context?.Dispose();
                DatabaseService.UnloadConfiguration();
            }
        }

        protected void OnCreateAccount()
        {
            var viewModel = new CreateAccountViewModel();
            viewModel.ShowDialog();

            this._account.Username = viewModel.Account.Username;
            this._account.Password = viewModel.Account.Password;
            this._account.PasswordConfirmation = viewModel.Account.PasswordConfirmation;
            this._account.Type = viewModel.Account.Type;
            this._account.IsValid = viewModel.Account.IsValid;
        }
    }
}
