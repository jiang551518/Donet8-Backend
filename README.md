# 项目名称

## 功能特性

- 使用 EPPlus 实现通用导入导出 Excel  
- 新增 MinIO 文件服务，用于文件上传与下载

## 环境依赖

- .NET 版本：.Net8  
- EPPlus：7.5.1  
- MinIO 服务

## MinIO 安装与启动

1. 下载可执行文件  
Invoke-WebRequest -Uri "https://dl.min.io/server/minio/release/windows-amd64/minio.exe" -OutFile "minio.exe"

2. 设置根用户并启动服务  
set MINIO_ROOT_USER=minioadmin
set MINIO_ROOT_PASSWORD=minioadmin
.\minio.exe server C:\minio-data --console-address ":9001"

3. 浏览器访问控制台  
http://127.0.0.1:9000

## 注册为 Windows 服务
如果已存在同名服务，先删除：
sc.exe delete MinIO

创建并设置为自动启动
sc.exe create MinIO binPath= ""C:\Users\Administrator\Downloads\minio.exe" server C:\minio-data --console-address ":9001"" start= auto
