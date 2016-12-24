// <copyright file="ObservableViewModelCollection`TViewModel`TModel.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// This class keeps an observable collection of ViewModel elements in sync with an observable collection of model elements.
    /// </summary>
    /// <typeparam name="TViewModel">The data type of the ViewModel for a model element of type TModel.</typeparam>
    /// <typeparam name="TModel">The data type of the Model objects this ViewModel collection is mirroring.</typeparam>
    public class ObservableViewModelCollection<TViewModel, TModel> : ObservableCollection<TViewModel>, IObservableViewModelCollection
        where TViewModel : class
        where TModel : class
    {
        private readonly Func<TModel, TViewModel> _viewModelFactory;
        private readonly Action<TViewModel> _viewModelRemoved;
        private INotifyCollectionChanged _modelCollection;

        /// <summary>
        /// Initializes a new instance of an ObservableViewModelCollection.
        /// </summary>
        /// <param name="source">The collection of model objects to mirror as ViewModels.</param>
        /// <param name="viewModelFactory">The factory method to use to create an appropriate ViewModel for each Model object.</param>
        /// <param name="viewModelRemoved">If any special action is required when a ViewModel is removed from the collection, supply a non-<c>null</c> action to call upon item removal.</param>
        public ObservableViewModelCollection(INotifyCollectionChanged source, Func<TModel, TViewModel> viewModelFactory, Action<TViewModel> viewModelRemoved)
            : base((source as IList).Cast<TModel>().Select(model => viewModelFactory(model)))
        {
            _modelCollection = source;
            _viewModelFactory = viewModelFactory;
            _viewModelRemoved = viewModelRemoved;
            _modelCollection.CollectionChanged += OnModelCollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of an ObservableViewModelCollection.
        /// </summary>
        /// <param name="viewModelFactory">The factory method to use to create an appropriate ViewModel for each Model object.</param>
        /// <param name="viewModelRemoved">If any special action is required when a ViewModel is removed from the collection, supply a non-<c>null</c> action to call upon item removal.</param>
        public ObservableViewModelCollection(Func<TModel, TViewModel> viewModelFactory, Action<TViewModel> viewModelRemoved)
        {
            _viewModelFactory = viewModelFactory;
            _viewModelRemoved = viewModelRemoved;
        }

        /// <summary>
        /// Gets or sets the source collection to observe / reflect into the ViewModel version of the collection.
        /// </summary>
        public IList SourceCollection
        {
            get
            {
                return _modelCollection as IList;
            }

            set
            {
                if (_modelCollection != value)
                {
                    if (_modelCollection != null)
                    {
                        _modelCollection.CollectionChanged -= OnModelCollectionChanged;
                        this.Clear();
                    }
                    _modelCollection = value as INotifyCollectionChanged;
                    if (value != null)
                    {
                        foreach (var item in value)
                        {
                            this.Add(CreateViewModel(item as TModel));
                        }
                    }
                    if (_modelCollection != null)
                    {
                        _modelCollection.CollectionChanged += OnModelCollectionChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the model collection as an observable collection.
        /// </summary>
        public ObservableCollection<TModel> ModelCollection
        {
            get { return _modelCollection as ObservableCollection<TModel>; }
        }

        /// <summary>
        /// Creates a ViewModel for a given Model object.
        /// </summary>
        /// <param name="model">The model object for which a view model is needed.</param>
        /// <returns>The view model for the model.</returns>
        protected virtual TViewModel CreateViewModel(TModel model)
        {
            return _viewModelFactory(model);
        }

        private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!INTV.Shared.Utility.OSDispatcher.IsMainThread)
            {
                // Would it be safe to BeginInvoke? Unsure about the order in which such things would execute.
                INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() => OnModelCollectionChanged(sender, e));
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        Insert(e.NewStartingIndex + i, CreateViewModel(e.NewItems[i] as TModel));
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (e.OldItems.Count == 1)
                    {
                        Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else
                    {
                        List<TViewModel> items = this.Skip(e.OldStartingIndex).Take(e.OldItems.Count).ToList();
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            RemoveViewModelAt(e.OldStartingIndex);
                        }
                        for (int i = 0; i < items.Count; i++)
                        {
                            Insert(e.NewStartingIndex + i, items[i]);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        RemoveViewModelAt(e.OldStartingIndex);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // remove
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        RemoveViewModelAt(e.OldStartingIndex);
                    }

                    // add
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        Insert(e.NewStartingIndex + i, CreateViewModel(e.NewItems[i] as TModel));
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    if (e.NewItems != null)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            Add(CreateViewModel(e.NewItems[i] as TModel));
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void RemoveViewModelAt(int index)
        {
            var viewModel = this.Items[index];
            RemoveAt(index);
            if ((viewModel != null) && (_viewModelRemoved != null))
            {
                _viewModelRemoved(viewModel);
            }
        }
    }
}
