using MvvmHelpers;
using PCLStorage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace ITvitaeChat2.ViewModel
{
    public class FolderIOViewModel : ViewModelBase
    {
        #region Variables
        IFolder rootFolder;
        #endregion

        #region Properties
        private ObservableCollection<File> allFiles;
        public ObservableCollection<File> AllFiles 
        {
            get => allFiles;
            set => SetProperty(ref allFiles, value);
        }
        #endregion

        // Default constructor
        public FolderIOViewModel()
        {
            rootFolder = FileSystem.Current.LocalStorage;

        }

        //private async ObservableCollection<File> GetFiles()
        //{
        //    //Get File list
        //    IList<IFile> files = await rootFolder.GetFilesAsync();

        //    //Get all File Name as string data
        //    IEnumerator filesList = files.GetEnumerator();
        //    int i = 0;
        //    string[] fileName = new string[10];
        //    fileName[i] = "Files in the Root Folder:"; i++;
        //    while (filesList.MoveNext())
        //    {
        //        IFile val = (IFile)filesList.Current;
        //        fileName[i] = "     - " + val.Name;
        //        i++;
        //    }
        //}
    }

    public class File : ObservableObject
    {
        private string fileName;
        public string FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);
        }
    }
}
