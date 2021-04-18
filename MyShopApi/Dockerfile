#UseUrls("http://*:5000")表示设置当前应用端口是5000
#若没有在Program中配置指定端口，例如：UseUrls("http://*:5000")，则容器默认80端口
#EXPOSE 80 在这里其实没有任何作用，在生成容器时还要看有没有配置UseUrls
#FROM swr.cn-south-1.myhuaweicloud.com/mcr/aspnet:3.1-alpine

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
COPY . .
EXPOSE 80            
ENTRYPOINT ["dotnet", "MyShopApi.dll"]