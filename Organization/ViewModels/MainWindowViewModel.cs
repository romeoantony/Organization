using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Organization.Models;
using Microsoft.EntityFrameworkCore;

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
            AddEmployeeCommand = new RelayCommand(_ => AddOrUpdateEmployee());
            CancelAddEmployeeCommand = new RelayCommand(_ => CancelAddOrEdit());
            EditEmployeeCommand = new RelayCommand(EditEmployee);
            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee);

            LoadEmployeesFromDatabase();
        }

        private void LoadEmployeesFromDatabase()
        {
            using var db = new OrganizationDbContext();
            db.Database.EnsureCreated();
            Employees.Clear();
            foreach (var emp in db.Employees.ToList())
                Employees.Add(emp);
        }

        private void StartAddEmployee()
        {
            EditingEmployee = null;
            NewEmployeeName = string.Empty;
            NewEmployeePosition = string.Empty;
            NewEmployeeSalary = string.Empty;
            IsAddEmployeeFormVisible = true;
            OnPropertyChanged(nameof(NewEmployeeName));
            OnPropertyChanged(nameof(NewEmployeePosition));
            OnPropertyChanged(nameof(NewEmployeeSalary));
        }

        private void AddOrUpdateEmployee()
        {
            if (string.IsNullOrWhiteSpace(NewEmployeeName)) return;
            decimal.TryParse(NewEmployeeSalary, out var salary);

            using var db = new OrganizationDbContext();
            if (EditingEmployee != null)
            {
                // Update existing employee
                EditingEmployee.Name = NewEmployeeName;
                EditingEmployee.Position = NewEmployeePosition;
                EditingEmployee.Salary = salary;
                db.Employees.Update(EditingEmployee);
                db.SaveChanges();
                EditingEmployee = null;
            }
            else
            {
                // Add new employee
                var emp = new Employee { Name = NewEmployeeName, Position = NewEmployeePosition, Salary = salary };
                db.Employees.Add(emp);
                db.SaveChanges();
                Employees.Add(emp);
            }

            LoadEmployeesFromDatabase();
            NewEmployeeName = string.Empty;
            NewEmployeePosition = string.Empty;
            NewEmployeeSalary = string.Empty;
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
                NewEmployeeSalary = emp.Salary.ToString();
                IsAddEmployeeFormVisible = true;
                OnPropertyChanged(nameof(NewEmployeeName));
                OnPropertyChanged(nameof(NewEmployeePosition));
                OnPropertyChanged(nameof(NewEmployeeSalary));
            }
        }

        private void DeleteEmployee(object? parameter)
        {
            if (parameter is Employee emp)
            {
                using var db = new OrganizationDbContext();
                db.Employees.Remove(emp);
                db.SaveChanges();
                Employees.Remove(emp);
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
