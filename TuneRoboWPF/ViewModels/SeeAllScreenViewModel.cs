using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using TuneRoboWPF.Models;
using TuneRoboWPF.StoreService;
using TuneRoboWPF.StoreService.SimpleRequest;
using TuneRoboWPF.Views;
using motion;

namespace TuneRoboWPF.ViewModels
{
    public class SeeAllScreenViewModel : ViewModelBase
    {
        private SeeAllScreenModel model = new SeeAllScreenModel();
        public CategoryMotionRequest.Type CategoryType { get; set; }
        public string Category
        {
            get { return model.Category; }
            set
            {
                model.Category = value;
                NotifyPropertyChanged("Category");
                switch (value)
                {
                    case "Hot":
                        CategoryType = CategoryMotionRequest.Type.ALL;
                        break;
                    case "Featured":
                        CategoryType=CategoryMotionRequest.Type.FEATURE;
                        break;
                }
            }
        }
        public ObservableCollection<MotionItemVertical> CategoryList
        {
            get { return model.CategoryList; }
            set
            {
                model.CategoryList = value;
                NotifyPropertyChanged("CategoryList");
            }
        }

        public bool NoResultVisibility
        {
            get { return model.NoResultVisibility; }
            set
            {
                if (model.NoResultVisibility == value) return;
                model.NoResultVisibility = value;
                NotifyPropertyChanged("NoResultVisibility");
            }
        }

        public string NoResultText
        {
            get { return string.Format("{0} {1}", Application.Current.TryFindResource("NoMotionInCategoryText"), model.Category); }
            set
            {
                model.NoResultText = value;
                NotifyPropertyChanged("NoResultText");
            }

        }
    }
}
