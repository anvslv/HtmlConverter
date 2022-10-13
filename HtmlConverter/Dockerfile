# Restore  
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Protos/convert.proto", "./Protos/"]
COPY ["HtmlConverter/HtmlConverter.csproj", "./HtmlConverter/"]
RUN dotnet restore "./HtmlConverter/HtmlConverter.csproj"

# Build  
COPY ["HtmlConverter/.", "./HtmlConverter/"]
WORKDIR "/src/."
RUN dotnet build "HtmlConverter/HtmlConverter.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "HtmlConverter/HtmlConverter.csproj" -c Release -o /app/publish

# Prepare runtime and app 
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app 
COPY --from=publish /app/publish .

ARG APP_PORT=5001
ENV ASPNETCORE_URLS http://*:$APP_PORT
EXPOSE $APP_PORT

# Start  
ENTRYPOINT ["dotnet", "HtmlConverter.dll"]