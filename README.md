# 项目名称
DoNET8的后端代码（根据自己经验写的demo版）
## 功能特性

- 使用 EPPlus 实现通用导入导出 Excel  
- 新增 MinIO 文件服务，用于文件上传与下载
- 实现使用elk查看接口调用日志
- 使用Dapper,EF Core和SqlSugar ORM来对数据库进行crud操作
- 实现通过工厂模式，判断查询数据库时用哪个orm查询
- 使用mapster进行类与类之间的映射

## 环境依赖

- DoNET 版本：.Net8  
- EPPlus：7.5.1  
- MinIO 服务
- elasticsearch-9.1.4
- kibana-9.1.4
- logstash-9.1.4
- jdk-25_windows-x64
  
（后续很多nuget包版本就不一一说了）

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


<img width="1920" height="953" alt="1758711230873_1baac770ac0c4d9b8ce0184fec32aee6" src="https://github.com/user-attachments/assets/9c65d3ed-ec8a-4b80-bbe4-5de18f4865c6" />

## elk配置服务（win环境）
下载链接：https://www.123912.com/s/FR8yVv-rxGOd 提取码：cEP9（自己折腾后成功了打包的）

在C:\elk\elasticsearch-9.1.4\bin 中用cmd执行：elasticsearch.bat 打开http://localhost:9200/ 可以访问，有json输出即正常

在C:\elk\kibana-9.1.4\bin 中用cmd执行：kibana.bat 打开http://localhost:5601/ 可以访问

在C:\elk\logstash-9.1.4\bin 中用cmd执行 logstash.bat -f config\logstash.conf 

都启动后 打开http://localhost:5601 即可进入到elk日志管理页面，只不过需要放开9200/5601/5000端口，也可以关闭防火墙


<img width="1920" height="953" alt="05fba9aa989edc8848a7263b98875fb4" src="https://github.com/user-attachments/assets/7a7c1052-7157-47d7-9d0a-efd4a8ffc331" />
