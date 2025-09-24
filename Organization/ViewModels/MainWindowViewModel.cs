using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Organization.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;

namespace Organization.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        private bool _isAddEmployeeFormVisible;
        public bool IsAddEmployeeFormVisible
        {
            get => _isAddEmployeeFormVisible;
            set => SetProperty(ref _isAddEmployeeFormVisible, value);
        }

        public string NewEmployeeName { get; set; } = string.Empty;
        public string NewEmployeePosition { get; set; } = string.Empty;
        public string NewEmployeeSalary { get; set; } = string.Empty;

        private string _formErrorMessage = string.Empty;
        public string FormErrorMessage
        {
            get => _formErrorMessage;
            set => SetProperty(ref _formErrorMessage, value);
        }

        public ICommand ShowAddEmployeeFormCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand CancelAddEmployeeCommand { get; }
        public ICommand EditEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        private Employee? _editingEmployee;
        public Employee? EditingEmployee
        {
            get => _editingEmployee;
            set => SetProperty(ref _editingEmployee, value);
        }

        public MainWindowViewModel()
        {
            ShowAddEmployeeFormCommand = new RelayCommand(_ => StartAddEmployee());
            AddEmployeeCommand = new RelayCommand(async _ => await AddOrUpdateEmployeeAsync());
            CancelAddEmployeeCommand = new RelayCommand(_ => CancelAddOrEdit());
            EditEmployeeCommand = new RelayCommand(EditEmployee);
            DeleteEmployeeCommand = new RelayCommand(async param => await DeleteEmployeeAsync(param));

            _ = LoadEmployeesFromDatabaseAsync();
        }

        private async Task LoadEmployeesFromDatabaseAsync()
        {
            using var db = new OrganizationDbContext();
            await db.Database.EnsureCreatedAsync();
            var list = await db.Employees.ToListAsync();
            Employees.Clear();
            foreach (var emp in list)
                Employees.Add(emp);
        }

        private void StartAddEmployee()
        {
            EditingEmployee = null;
            NewEmployeeName = string.Empty;
            NewEmployeePosition = string.Empty;
            NewEmployeeSalary = string.Empty;
            FormErrorMessage = string.Empty;
            IsAddEmployeeFormVisible = true;
            OnPropertyChanged(nameof(NewEmployeeName));
            OnPropertyChanged(nameof(NewEmployeePosition));
            OnPropertyChanged(nameof(NewEmployeeSalary));
        }

        private async Task AddOrUpdateEmployeeAsync()
        {
            FormErrorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(NewEmployeeName))
            {
                FormErrorMessage = "Name is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewEmployeePosition))
            {
                FormErrorMessage = "Position is required.";
                return;
            }
            if (!decimal.TryParse(NewEmployeeSalary, NumberStyles.Number, CultureInfo.InvariantCulture, out var salary) || salary <= 0)
            {
                FormErrorMessage = "Salary must be a positive number.";
                return;
            }

            using var db = new OrganizationDbContext();
            if (EditingEmployee != null)
            {
                // Fetch tracked entity and update
                var tracked = await db.Employees.FindAsync(EditingEmployee.Id);
                if (tracked != null)
                {
                    tracked.Name = NewEmployeeName;
                    tracked.Position = NewEmployeePosition;
                    tracked.Salary = salary;
                    await db.SaveChangesAsync();
                }
                EditingEmployee = null;
            }
            else
            {
                // Add new employee
                var emp = new Employee { Name = NewEmployeeName, Position = NewEmployeePosition, Salary = salary };
                db.Employees.Add(emp);
                await db.SaveChangesAsync();
            }

            await LoadEmployeesFromDatabaseAsync();
            NewEmployeeName = string.Empty;
            NewEmployeePosition = string.Empty;
            NewEmployeeSalary = string.Empty;
            FormErrorMessage = string.Empty;
            IsAddEmployeeFormVisible = false;
            OnPropertyChanged(nameof(NewEmployeeName));
            OnPropertyChanged(nameof(NewEmployeePosition));
            OnPropertyChanged(nameof(NewEmployeeSalary));
        }

        private void CancelAddOrEdit()
        {
            EditingEmployee = null;
            NewEmployeeName = string.Empty;
            NewEmployeePosition = string.Empty;
            NewEmployeeSalary = string.Empty;
            FormErrorMessage = string.Empty;
            IsAddEmployeeFormVisible = false;
            OnPropertyChanged(nameof(NewEmployeeName));
            OnPropertyChanged(nameof(NewEmployeePosition));
            OnPropertyChanged(nameof(NewEmployeeSalary));
        }

        private void EditEmployee(object? parameter)
        {
            if (parameter is Employee emp)
            {
                EditingEmployee = emp;
                NewEmployeeName = emp.Name;
                NewEmployeePosition = emp.Position;
                NewEmployeeSalary = emp.Salary.ToString(CultureInfo.InvariantCulture);
                FormErrorMessage = string.Empty;
                IsAddEmployeeFormVisible = true;
                OnPropertyChanged(nameof(NewEmployeeName));
                OnPropertyChanged(nameof(NewEmployeePosition));
                OnPropertyChanged(nameof(NewEmployeeSalary));
            }
        }

        private async Task DeleteEmployeeAsync(object? parameter)
        {
            if (parameter is Employee emp)
            {
                using var db = new OrganizationDbContext();
                var tracked = await db.Employees.FindAsync(emp.Id);
                if (tracked != null)
                {
                    db.Employees.Remove(tracked);
                    await db.SaveChangesAsync();
                }
                await LoadEmployeesFromDatabaseAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
    }
}
