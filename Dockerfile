# Build the runtime image
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# FROM public.ecr.aws/lambda/dotnet:7 AS build
# FROM public.ecr.aws/bitnami/dotnet-sdk:7 as build
# FROM public.ecr.aws/h0u6n5q5/sdk-prod:313bfc990646b1c8651c0_313bfc990646b1c8651c0 AS build
FROM public.ecr.aws/y7n1s6r0/trading-dev-core-sdk:7.0 as build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
FROM public.ecr.aws/kbxt/dotnet/aspnet:7.0 AS runtime
EXPOSE 80
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "testpdf.dll"]
