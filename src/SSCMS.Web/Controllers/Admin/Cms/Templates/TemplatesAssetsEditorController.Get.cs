﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (request.FileType == "html")
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesIncludes))
                {
                    return Unauthorized();
                }
            }
            else
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
                {
                    return Unauthorized();
                }
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var path = string.Empty;
            var content = string.Empty;

            var directoryPath = PathUtils.RemoveParentPath(request.DirectoryPath);
            var fileName = PathUtils.RemoveParentPath(request.FileName);

            if (!string.IsNullOrEmpty(fileName))
            {
                var filePath = await _pathManager.GetSitePathAsync(site, directoryPath, fileName);
                
                if (FileUtils.IsFileExists(filePath))
                {
                    content = await FileUtils.ReadTextAsync(filePath);
                }

                if (StringUtils.EqualsIgnoreCase(request.FileType, "html"))
                {
                    path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(directoryPath, site.TemplatesAssetsIncludeDir,
                        string.Empty), fileName);
                }
                else if (StringUtils.EqualsIgnoreCase(request.FileType, "css"))
                {
                    path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(directoryPath, site.TemplatesAssetsCssDir,
                        string.Empty), fileName);
                }
                else if (StringUtils.EqualsIgnoreCase(request.FileType, "js"))
                {
                    path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(directoryPath, site.TemplatesAssetsJsDir,
                        string.Empty), fileName);
                }

                path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path));
            }

            return new GetResult
            {
                TemplatesAssetsIncludeDir = site.TemplatesAssetsIncludeDir,
                TemplatesAssetsCssDir = site.TemplatesAssetsCssDir,
                TemplatesAssetsJsDir = site.TemplatesAssetsJsDir,
                Path = path,
                Content = content
            };
        }
    }
}
