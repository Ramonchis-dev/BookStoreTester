# Stage 1: Build the Blazor application
# Uses the .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container to the application's root.
# Assuming Render's "Root Directory" for the service is set to 'BookStoreTester/'.
WORKDIR /app

# Copy the .csproj file to leverage Docker layer caching.
# The .csproj file is expected to be in the root of the build context (which is 'BookStoreTester/').
COPY *.csproj ./

# Restore project dependencies.
# 'dotnet restore' will find the .csproj in the current WORKDIR (/app).
RUN dotnet restore

# Copy the rest of the application's source code.
# The .dockerignore file (located in 'BookStoreTester/') will prevent 'Dockerfile' itself
# from being copied into '/app', preventing it from being accidentally compiled as C# code.
COPY . .

# Publish the application to an 'out' directory within the current WORKDIR (/app).
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
# Uses the ASP.NET runtime image for a smaller, production-ready image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime stage.
WORKDIR /app

# Copy the published output from the 'build' stage.
# The 'out' directory from the previous stage is copied to '/app' in this stage.
COPY --from=build /app/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render.
# Render automatically sets the $PORT environment variable.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts.
# 'BookStoreTester.dll' is the name of your compiled application DLL.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]
