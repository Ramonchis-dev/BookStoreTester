# Stage 1: Build the Blazor application
# Uses the .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container to a temporary build directory.
# This is a common practice to isolate build artifacts.
WORKDIR /src

# Copy the specific .csproj file for your application to a subfolder.
# This assumes your Dockerfile is in the 'BookStoreTester/' directory on your host,
# and that 'BookStoreTester/' is set as Render's 'Root Directory'.
# So, the build context root is 'BookStoreTester/'.
# We copy BookStoreTester.csproj from the build context root into /src/BookStoreTester/ in the container.
COPY BookStoreTester.csproj ./BookStoreTester/

# Restore project dependencies for the specific .csproj file.
# We explicitly tell 'dotnet restore' which project file to process,
# ensuring it finds it in the /src/BookStoreTester/ directory within the container.
RUN dotnet restore ./BookStoreTester/BookStoreTester.csproj

# Copy the rest of the application's source code from the build context.
# This will copy all other files from 'BookStoreTester/' on your host to '/src/' in the container.
# The .dockerignore file (located in 'BookStoreTester/') will prevent 'Dockerfile' itself
# from being copied into '/src', preventing it from being accidentally compiled as C# code.
COPY . .

# Set the working directory to the specific project directory where 'dotnet publish' will run.
# This ensures 'dotnet publish' has access to all necessary project files.
WORKDIR /src/BookStoreTester

# Publish the application to an 'out' directory within the current WORKDIR.
# The published output will be located at /src/BookStoreTester/out in this stage.
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
# Uses the ASP.NET runtime image for a smaller, production-ready image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime stage.
WORKDIR /app

# Copy the published output from the 'build' stage to the final runtime location.
# We copy from the explicit path where it was created in the build stage.
COPY --from=build /src/BookStoreTester/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render.
# Render automatically sets the $PORT environment variable for web services.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts.
# 'BookStoreTester.dll' is the name of your compiled application DLL.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]
