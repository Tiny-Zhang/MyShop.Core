#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
#WORKDIR /app
#EXPOSE 80

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["MyShopApi.csproj", ""]
#RUN dotnet restore "./MyShopApi.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "MyShopApi.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "MyShopApi.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "MyShopApi.dll"]

#UseUrls("http://*:5000")表示设置当前应用端口是5000
#若没有在Program中配置指定端口，例如：UseUrls("http://*:5000")，则容器默认80端口
#EXPOSE 80 在这里其实没有任何作用，在生成容器时还要看有没有配置UseUrls
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
FROM swr.cn-south-1.myhuaweicloud.com/mcr/aspnet:3.1-alpine
WORKDIR /app
COPY . .
EXPOSE 80            
ENTRYPOINT ["dotnet", "MyShopApi.dll"]
