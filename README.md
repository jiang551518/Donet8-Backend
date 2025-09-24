# 项目名称
donet8版本的后端代码（基础版）
## 功能特性

- 使用 EPPlus 实现通用导入导出 Excel  
- 新增 MinIO 文件服务，用于文件上传与下载
- 实现使用elk查看接口调用日志

## 环境依赖

- .NET 版本：.Net8  
- EPPlus：7.5.1  
- MinIO 服务
- elasticsearch-9.1.4
- kibana-9.1.4
- logstash-9.1.4
- jdk-25_windows-x64

## MinIO 安装与启动

1. 下载可执行文件（也可以手动下载）
Invoke-WebRequest -Uri "[https://dl.min.io/server/minio/release/windows-amd64](https://dl.min.io/server/minio/release/windows-amd64/)/minio.exe" -OutFile "minio.exe"

2. 设置根用户并启动服务：  
set MINIO_ROOT_USER=minioadmin
set MINIO_ROOT_PASSWORD=minioadmin
.\minio.exe server C:\minio-data --console-address ":9001"

3. 浏览器访问控制台：  
http://127.0.0.1:9000

4. 临时启动命令：  
.\minio.exe server C:\minio-data --console-address ":9001"

## 注册为 Windows 服务
如果已存在同名服务，先删除：
sc.exe delete MinIO

创建并设置为自动启动：
sc.exe create MinIO binPath= ""C:\Users\Administrator\Downloads\minio.exe" server C:\minio-data --console-address ":9001"" start= auto

## elk配置服务（win环境）

在C:\elk\elasticsearch-9.1.4\bin 中用cmd执行：elasticsearch.bat 打开http://localhost:9200/可以访问，有json输出即正常

在C:\elk\kibana-9.1.4\bin 中用cmd执行：kibana.bat 打开http://localhost:5601/可以访问

在C:\elk\logstash-9.1.4\bin 中用cmd执行 logstash.bat -f config\logstash.conf 

都启动后 打开http://localhost:5601即可进入到elk日志管理页面，只不过需要放开9200/5601/5000端口，也可以关闭防火墙
