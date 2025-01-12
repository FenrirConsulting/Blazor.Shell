# Blazor.Shell

## Overview
Blazor.Shell serves as the central hosting environment for the Blazor Enterprise Architecture. It provides a container interface that allows multiple Blazor applications to be mounted and displayed within a unified window, creating a seamless user experience across different modules.

## Features
- 🖼️ Application hosting container
- 🔄 Dynamic module loading
- 📱 Responsive layout system
- 🔐 Integrated authentication
- 📊 Performance monitoring
- 🎨 Unified theming

## Architecture
The Shell application serves as:
- Central navigation hub
- Authentication checkpoint
- Module container
- State coordinator
- Resource manager

## Module Integration

### Module Registration
```csharp
services.AddBlazorModule(options => {
    options.Name = "MyModule";
    options.EntryPoint = "/module-path";
    options.RequiresAuth = true;
});
```

### Module Mounting
```razor
<ModuleContainer>
    <ModuleHost Name="@moduleName" />
</ModuleContainer>
```

## Configuration

### appsettings.json
```json
{
  "Shell": {
    "ModuleSettings": {
      "LoadTimeout": 30,
      "RetryAttempts": 3,
      "CacheDuration": 3600
    },
    "Authentication": {
      "RequireAuth": true,
      "AuthEndpoint": "https://auth.yourdomain.com"
    }
  }
}
```

## Setup Instructions

### 1. Prerequisites
- .NET 7.0+
- IIS Server
- SQL Server
- Azure AD tenant

### 2. Installation
1. Clone repository
2. Configure settings
3. Build solution
4. Deploy to IIS
5. Configure modules

### 3. Module Configuration
```xml
<modules>
  <module name="Module1" path="/module1" />
  <module name="Module2" path="/module2" />
</modules>
```

## Security Features
- Authentication integration
- Module isolation
- Resource protection
- Session management
- Audit logging

## Performance Optimization
- Module lazy loading
- Resource caching
- State management
- Memory optimization
- Load balancing

## Development Guide

### Creating New Modules
1. Use Blazor.Module template
2. Implement required interfaces
3. Configure module settings
4. Register with shell
5. Deploy module

### Testing
1. Unit tests for shell functions
2. Integration tests for modules
3. Performance testing
4. Security testing
5. UI/UX testing

## IIS Deployment

### Web.config
```xml
<configuration>
  <system.webServer>
    <modules>
      <remove name="WebDAVModule" />
    </modules>
    <handlers>
      <remove name="WebDAV" />
    </handlers>
  </system.webServer>
</configuration>
```

## Monitoring
- Application performance
- Module health
- User sessions
- Resource usage
- Error tracking

## Troubleshooting
1. Module Loading Issues
   - Check module registration
   - Verify paths
   - Check permissions
2. Authentication Problems
   - Verify configuration
   - Check tokens
   - Review logs

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author
Christopher Olson - Senior Security Engineer