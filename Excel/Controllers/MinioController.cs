using Excel.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace Excel.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MinioController
    {
        private readonly IMinioClient _minioClient;
        public MinioController(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        /// <summary>
        /// 文件上传到minio（返回url）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost(nameof(Upload))]
        public async Task<ActionResult<UploadResult>> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("文件不能为空");

            var bucket = "mybucket";
            var objectName = file.FileName;

            // 1. 检查桶是否存在
            bool exists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket)
            ).ConfigureAwait(false);

            if (!exists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(bucket)
                ).ConfigureAwait(false);
            }

            using var stream = file.OpenReadStream();
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
            ).ConfigureAwait(false);

            var url = await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithExpiry(60 * 60 * 24)
            ).ConfigureAwait(false);

            return new UploadResult { Url = url };
        }
    }
}
