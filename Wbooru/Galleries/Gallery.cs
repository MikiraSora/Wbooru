﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wbooru.Galleries.SupportFeatures;
using Wbooru.Models;
using Wbooru.Models.Gallery;
using Wbooru.PluginExt;
using Wbooru.Settings;

namespace Wbooru.Galleries
{
    public abstract class Gallery
    {
        #region SupportFeatures

        private GallerySupportFeature? _support = null;
        public GallerySupportFeature SupportFeatures => (_support ?? (_support = _CheckGalleryFeatures())) ?? default;
        private GallerySupportFeature _CheckGalleryFeatures()
        {
            GallerySupportFeature support = default;

            if (this is IGalleryMark)
                support |= GallerySupportFeature.Mark;

            if (this is IGalleryVote)
                support |= GallerySupportFeature.Vote;

            if (this is IGalleryAccount)
                support |= GallerySupportFeature.Account;

            if (this is IGalleryTagSearch)
                support |= GallerySupportFeature.TagSearch;

            if (this is IGalleryTagDataPredownloadAndCache)
                support |= GallerySupportFeature.TagDataPredownloadAndCache;

            if (this is IGallerySearchImage)
                support |= GallerySupportFeature.ImageSearch;

            if (this is IGalleryItemIteratorFastSkipable)
                support |= GallerySupportFeature.ImageFastSkipable;

            if (this is IGalleryNSFWFilter)
                support |= GallerySupportFeature.NSFWFilter;

            return support;
        }

        public T Feature<T>() where T : class, IGalleryFeature => this as T;

        #endregion

        public abstract string GalleryName { get; }

        public abstract IAsyncEnumerable<GalleryItem> GetMainPostedImagesAsync();

        public abstract Task<GalleryImageDetail> GetImageDetialAsync(GalleryItem item);

        public abstract Task<GalleryItem> GetImageAsync(string id);

        #region Helper Methods

        internal IAsyncEnumerable<GalleryItem> TryFilterIfNSFWEnable(IAsyncEnumerable<GalleryItem> item)
        {
            if (!SettingManager.LoadSetting<GlobalSetting>().EnableNSFWFileterMode)
                return item;

            if (Feature<IGalleryNSFWFilter>() is IGalleryNSFWFilter filter)
                return filter.NSFWFilter(item);

            throw new Exception("Not allow to use if galllery not support NSFW fileter and EnableNSFWFileterMode = true ");
        }

        #endregion
    }
}
