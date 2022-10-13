# Restore  
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Protos/convert.proto", "./Protos/"]
COPY ["HtmlConverter.ConversionService/HtmlConverter.ConversionService.csproj", "./HtmlConverter.ConversionService/"]
RUN dotnet restore "./HtmlConverter.ConversionService/HtmlConverter.ConversionService.csproj"

# Build  
COPY ["HtmlConverter.ConversionService/.", "./HtmlConverter.ConversionService/"]
WORKDIR "/src/."
RUN dotnet build "HtmlConverter.ConversionService/HtmlConverter.ConversionService.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "HtmlConverter.ConversionService/HtmlConverter.ConversionService.csproj" -c Release -o /app/publish

# Prepare runtime and app 
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app 
COPY --from=publish /app/publish .

ARG APP_PORT=5002
ENV ASPNETCORE_URLS http://*:$APP_PORT
EXPOSE $APP_PORT

# Install puppeteer dependencies
# RUN apt-get update && apt-get install -y xorg openbox libnss3 libasound2
RUN apt-get update && apt-get install -y libx11-6 libx11-xcb1 libatk1.0-0 libgtk-3-0 libcups2 libdrm2 libxkbcommon0 libxcomposite1 libxdamage1 libxrandr2 libgbm1 libpango-1.0-0 libcairo2 libasound2 libxshmfence1 libnss3

# Start  
ENTRYPOINT ["dotnet", "HtmlConverter.ConversionService.dll"]
