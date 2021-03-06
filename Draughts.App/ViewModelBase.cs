﻿using Prism.Mvvm;
using Prism.Regions;

namespace Draughts.App
{
    internal class ViewModelBase : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual bool KeepAlive => false;
    }
}