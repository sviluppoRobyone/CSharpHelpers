﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpHelpers.Api.ResponseType
{
    public class FileResultFromPath : IHttpActionResult
    {
        private readonly string _filePath;
        private readonly string _contentType;
        public string Filename = null;
        public FileResultFromPath(string filePath, string contentType = null)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            _filePath = filePath;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(File.OpenRead(_filePath))
            };

            var contentType = _contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            if (!string.IsNullOrEmpty(Filename))
                response.Content.Headers.ContentDisposition =
                    new ContentDispositionHeaderValue("attachment") { FileName = Filename };

            return Task.FromResult(response);
        }
    }


    public class FileResultFromByteArray : IHttpActionResult
    {
        private readonly byte[] _fileData;
        private readonly string _contentType;
        public string Filename = null;
        public FileResultFromByteArray(byte[] fileData, string contentType)
        {
            if (fileData == null)
                throw new ArgumentNullException(nameof(fileData));

            _fileData = fileData;
            _contentType = contentType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream(_fileData))
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
            if (!string.IsNullOrEmpty(Filename))
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = Filename };
            return Task.FromResult(response);
        }
    }
}
