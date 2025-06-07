# Stage 1: Build the Blazor application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies first
# This step is optimized for Docker layer caching.
# If only source code changes, this layer can be reused.
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application to 'out' directory
# -c Release: Build in Release configuration
# -o out: Output to a directory named 'out'
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the 'build' stage
COPY --from=build /app/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render
# Render automatically sets the $PORT environment variable.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts
# Replace 'YourAppName.dll' with the actual name of your compiled application DLL.
# This is typically the name of your .csproj file with a .dll extension.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]# Stage 1: Build the Blazor application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies first
# This step is optimized for Docker layer caching.
# If only source code changes, this layer can be reused.
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . .

# Publish the application to 'out' directory
# -c Release: Build in Release configuration
# -o out: Output to a directory named 'out'
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the 'build' stage
COPY --from=build /app/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render
# Render automatically sets the $PORT environment variable.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts
# Replace 'YourAppName.dll' with the actual name of your compiled application DLL.
# This is typically the name of your .csproj file with a .dll extension.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]