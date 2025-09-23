# Organization Employee Manager

A WPF desktop application built with .NET 8 and MVVM architecture for managing employees. Supports CRUD operations with persistent storage using SQLite and Entity Framework Core.

## Features
- List employees in a card-style UI
- Add new employees with inline form
- Edit and delete employees
- Data persists in a local SQLite database
- MVVM pattern for clean separation of UI and logic

## Technologies Used
- .NET 8
- WPF
- MVVM
- Entity Framework Core (SQLite)

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Setup
1. Clone the repository:
   ```sh
   git clone <your-repo-url>
   ```
2. Navigate to the project directory:
   ```sh
   cd Organization
   ```
3. Restore NuGet packages:
   ```sh
   dotnet restore
   ```
4. Build and run the app:
   ```sh
   dotnet build
   dotnet run --project Organization/Organization.csproj
   ```

### Database
- The app uses a local SQLite database file (`organization.db`).
- Database schema is managed via Entity Framework Core migrations.

## Project Structure
- `Models/` - Data models and EF Core DbContext
- `ViewModels/` - MVVM ViewModels and commands
- `EmployeeCard.xaml` - Custom UserControl for employee display
- `MainWindow.xaml` - Main UI layout

## How to Use
- Click **Add Employee** to open the inline form.
- Fill in details and click **Save** to add or update.
- Use **Edit** and **Delete** buttons on each employee card.
- All changes are saved to the database and persist between sessions.

## License
MIT
