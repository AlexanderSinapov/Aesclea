# PostgreSQL Database Setup Guide

## Prerequisites
1. Install PostgreSQL 12 or later on your system
2. Ensure PostgreSQL service is running

## Database Setup

### 1. Create Database
Connect to PostgreSQL as superuser and run:

```sql
-- Create database
CREATE DATABASE aesclea_db;

-- Create user (optional, for security)
CREATE USER aesclea_user WITH PASSWORD 'your_secure_password';

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE aesclea_db TO aesclea_user;
```

### 2. Connect to the Database
```bash
psql -U postgres -d aesclea_db
```

### 3. Create Tables
The application will automatically create the required tables using Entity Framework's `EnsureCreated()` method when it starts.

However, if you want to create them manually:

```sql
-- Users table
CREATE TABLE users (
    id VARCHAR(255) PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    phone VARCHAR(50),
    role VARCHAR(50) NOT NULL,
    hospital VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Patients table
CREATE TABLE patients (
    id VARCHAR(255) PRIMARY KEY,
    first_name VARCHAR(255) NOT NULL,
    last_name VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    phone VARCHAR(50),
    date_of_birth DATE,
    gender VARCHAR(20),
    medical_history TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_patients_email ON patients(email);
```

## Configuration

Update your `appsettings.json` connection string:

### For local development:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=aesclea_db;Username=postgres;Password=your_password"
  }
}
```

### For production:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your_server;Database=aesclea_db;Username=aesclea_user;Password=your_secure_password;SSL Mode=Require"
  }
}
```

## Verification

1. Start the backend application
2. Check the console for "Database initialization completed successfully"
3. Test the API endpoints:
   - GET `/health` - Should return health status
   - GET `/api/test` - Should return test message
   - POST `/api/auth/register` - Try registering a new user

## Troubleshooting

### Connection Issues
- Ensure PostgreSQL is running: `sudo systemctl status postgresql`
- Check PostgreSQL logs: `tail -f /var/log/postgresql/postgresql-*.log`
- Verify connection string credentials

### Permission Issues
- Grant proper permissions to the user
- Check pg_hba.conf for authentication method

### SSL Issues in Development
Add `;SSL Mode=Disable` to connection string for local development only.
