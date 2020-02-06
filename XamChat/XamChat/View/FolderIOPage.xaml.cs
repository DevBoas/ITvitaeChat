using ITvitaeChat2.ViewModel;
using PCLStorage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITvitaeChat2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderIOPage : ContentPage
    {
        public FolderIOPage()
        {
            InitializeComponent();
            
            BindingContext = new FolderIOViewModel();

            Title = "File explorer";
        }

        //Get Root Folder Path
        IFolder rootFolder = FileSystem.Current.LocalStorage;

        //
        //  Folder I/O
        //
        //Display Root Folder's Name and Path
        void DisplayRootFolder(object sender, EventArgs e)
        {
            string[] RootFolderInfo = new string[]
            {
                "Root Folder Name:",
                "       - " + rootFolder.Name,
                "Root Folder Path:",
                "       - " + rootFolder.Path
            };
            DisplayResult(FolderIOResult, RootFolderInfo);
        }


        //Create a new Folder
        async void CreateFolder(object sender, EventArgs e)
        {
            var Entry = EntCreateFolder;

            // Alert, if no Folder Name
            if (Entry.Text == null)
            {
                await DisplayAlert("Create Subfolder", "Input Subfolder Name!", "OK");
                return;
            }

            // Create a new Folder named in EntCreateSubFolder
            IFolder subfolder =
                await rootFolder.CreateFolderAsync(Entry.Text, CreationCollisionOption.OpenIfExists);
            await DisplayAlert("Create Subfolder", Entry.Text + " has been created successfully!", "OK");
        }


        //Check Folder Existence
        async void CheckFolder(object sender, EventArgs e)
        {
            var Entry = EntCheckFolder;

            // Alert, if no Folder Name
            if (Entry.Text == null)
            {
                await DisplayAlert("Check Folder Result", "Input Folder Name!", "OK");
                return;
            }

            // Check Folder Existence entered in EntCheckSubFolder
            var subfolderExists = await rootFolder.CheckExistsAsync(Entry.Text);
            if (subfolderExists != ExistenceCheckResult.FolderExists)
            {
                await DisplayAlert("Check Folder Result", Entry.Text + " doesn't exist", "OK");
                return;
            }
            await DisplayAlert("Check Folder Result", Entry.Text + " exists!", "OK");
        }


        //Delete Folder
        async void DeleteFolder(object sender, EventArgs e)
        {
            var Entry = EntDeleteFolder;

            // Alert, if SubFolder Name was not entered
            if (Entry.Text == null)
            {
                await DisplayAlert("Delete Folder Result", "Input Folder Name!", "OK");
                return;
            }

            // Delete Folder entered in EntDeleteSubFolder
            var subfolderExists = await rootFolder.CheckExistsAsync(Entry.Text);
            if (subfolderExists != ExistenceCheckResult.FolderExists)
            {
                await DisplayAlert("Delete Folder Result", Entry.Text + " doesn't exist", "OK");
                return;
            }
            IFolder subfolder = await rootFolder.GetFolderAsync(Entry.Text);
            await subfolder.DeleteAsync();
            await DisplayAlert("Delete Folder Result", Entry.Text + " has been deleted!", "OK");
        }


        // Get & Display Folders list
        async void GetFolders(object sender, EventArgs e)
        {
            //Get the Folders list
            IList<IFolder> folders = await rootFolder.GetFoldersAsync();

            //Get all Folder name as string data
            IEnumerator foldersList = folders.GetEnumerator();
            int i = 0;
            string[] folderName = new string[10];
            folderName[i] = "Subfolders in the Root Folder:"; i++;
            while (foldersList.MoveNext())
            {
                IFolder val = (IFolder)foldersList.Current;
                folderName[i] = "     - " + val.Name;
                i++;
            }

            //The number of Folders
            folderName[i] = "//  " + folders.Count.ToString() + " subfolders";

            //Display Folder names and the number of them
            DisplayResult(FolderIOResult, folderName);
        }


        //
        //  Display & Clear Folder I/O Result
        //
        //Display the results in ListView
        void DisplayResult(ListView ListViewName, string[] result)
        {
            ListViewName.ItemsSource = result;
        }
        //Clear all the Folder I/O result
        void ClearFolderIOResult(object sender, EventArgs e)
        {
            FolderIOResult.ItemsSource = null;
        }
        //Display the tapped Folder
        void TappedResultItem(object sender, ItemTappedEventArgs e)
        {
            DisplayAlert("Item Tapped", e.Item.ToString(), "OK");
        }
    }
}