using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ReactNative.Managed;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.ReactNative.Managed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;


    namespace test
{
    [ReactModule]
     class FileUp
    {
        ReactContext context;

        [ReactInitializer]
        public void Initialize(ReactContext reactContext)
        {
            context = reactContext;
        }

        [ReactMethod("FileUpld")]
        public async Task<JSValueObject> FileUpld()
        {
            TaskCompletionSource<JSValueObject> tcs = new TaskCompletionSource<JSValueObject>();
            context.Handle.UIDispatcher.Post(async () =>
            {
                Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".txt");

          //  ((IInitializeWithWindow)(object)picker).Initialize(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
            //Prompt the user to open the log file:
            
                Windows.Storage.StorageFile logFile = await picker.PickSingleFileAsync();

                try
                {
                    if (logFile != null)
                    {
                        JSValueObject obj = new JSValueObject
                {
                    { "uri", logFile.Path }
                };

                         tcs.SetResult(obj);
                    }
                    
                    //using (var stream = await logFile.OpenStreamForReadAsync())
                    //using (var reader = new StreamReader(stream))
                    //{
                    //    var line = await reader.ReadLineAsync();
                    //    Debug.WriteLine($"The first line: {line} - waiting");
                    //    await Task.Delay(10000);
                    //    line = await reader.ReadLineAsync();
                    //    Debug.WriteLine($"The next line: {line} - waiting");
                    //}
                }
                catch (Exception exc)
                {
                    Debug.WriteLine($"Exception {exc.Message}");
                }
            });
            var result = await tcs.Task;
            return result;
            //context.Handle.UIDispatcher.Post(async () => {
            //            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //            // Other initialization code
            //            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            //            if (file != null)
            //            {
            //                // File opened successfully
            //            }
            //            else
            //            {
            //                // Error while opening the file
            //            }
            //        });
            //var openPicker = new FileOpenPicker();
            //StorageFile file = await openPicker.PickSingleFileAsync();
            //// Process picked file
            //if (file != null)
            //{
            //    // Store file for future access
            //    Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
            //}
            //else
            //{
            //    // The user didn't pick a file
            //}


        }





            private static readonly string OPTION_TYPE = "type";
            private static readonly string CACHE_TYPE = "cache";
            private static readonly string OPTION_MULTIPLE = "allowMultiSelection";
            private static readonly string OPTION_READ_CONTENT = "readContent";
            private static readonly string FIELD_URI = "uri";
            private static readonly string FIELD_FILE_COPY_URI = "fileCopyUri";
            private static readonly string FIELD_NAME = "name";
            private static readonly string FIELD_TYPE = "type";
            private static readonly string FIELD_SIZE = "size";
            private static readonly string FIELD_CONTENT = "content";



        
        public async Task<List<JSValueObject>> Pick()
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


                //var fileTypeArray = options.AsObject()[OPTION_TYPE][0].AsString();
                bool cache = false;

          
                bool readContent = false;


                //if pick called to launch folder picker.
                bool isFolderPicker = false;
   

                List<JSValueObject> result;
            result = await PickSingleFileAsync(openPicker, cache, readContent);

                return result;
            }

            [ReactMethod("pickDirectory")]
            public async Task<JSValueObject> PickDirectory()
            {
                TaskCompletionSource<JSValueObject> tcs = new TaskCompletionSource<JSValueObject>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var openFolderPicker = new FolderPicker();

                    openFolderPicker.ViewMode = PickerViewMode.List;
                    openFolderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    openFolderPicker.FileTypeFilter.Add("*");

                    var folder = await openFolderPicker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        JSValueObject obj = new JSValueObject
                {
                    { "uri", folder.Path }
                };

                        tcs.SetResult(obj);
                    }
                    else
                    {
                        tcs.SetResult(new JSValueObject());
                    }
                });

                var result = await tcs.Task;
                return result;
            }
            private async Task<JSValueObject> PrepareFile(StorageFile file, bool cache, bool readContent)
            {
                string base64Content = null;
                if (readContent)
                {
                    var fileStream = await file.OpenReadAsync();
                    using (StreamReader reader = new StreamReader(fileStream.AsStream()))
                    {
                        using (var memstream = new MemoryStream())
                        {
                            await reader.BaseStream.CopyToAsync(memstream);
                            var bytes = memstream.ToArray();
                            base64Content = Convert.ToBase64String(bytes);
                        }
                    }
                }

                if (cache == true)
                {
                    var fileInCache = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name.ToString(), NameCollisionOption.ReplaceExisting);
                    var basicProperties = await fileInCache.GetBasicPropertiesAsync();

                    JSValueObject result = new JSValueObject
                {
                    { FIELD_URI, file.Path },
                    { FIELD_FILE_COPY_URI, file.Path },
                    { FIELD_TYPE, file.ContentType },
                    { FIELD_NAME, file.Name },
                    { FIELD_SIZE, basicProperties.Size},
                    { FIELD_CONTENT, base64Content }
                };

                    return result;
                }
                else
                {
                    var basicProperties = await file.GetBasicPropertiesAsync();

                    JSValueObject result = new JSValueObject
                {
                    { FIELD_URI, file.Path },
                    { FIELD_FILE_COPY_URI, file.Path },
                    { FIELD_TYPE, file.ContentType },
                    { FIELD_NAME, file.Name },
                    { FIELD_SIZE, basicProperties.Size},
                    { FIELD_CONTENT, base64Content }
                };

                    return result;
                }
            }

            private async Task<List<JSValueObject>> PickMultipleFileAsync(FileOpenPicker picker, bool cache, bool readContent)
            {
                TaskCompletionSource<List<JSValueObject>> tcs = new TaskCompletionSource<List<JSValueObject>>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                    if (files.Count > 0)
                    {
                        List<JSValueObject> jarrayObj = new List<JSValueObject>();
                        foreach (var file in files)
                        {
                            var processedFile = await PrepareFile(file, cache, readContent);
                            jarrayObj.Add(processedFile);
                        }

                        tcs.SetResult(jarrayObj);
                    }
                    else
                    {
                        tcs.SetResult(new List<JSValueObject>());
                    }
                });

                var result = await tcs.Task;
                return result;
            }

            private async Task<List<JSValueObject>> PickSingleFileAsync(FileOpenPicker picker, bool cache, bool readContent)
            {
                TaskCompletionSource<JSValueObject> tcs = new TaskCompletionSource<JSValueObject>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var file = await picker.PickSingleFileAsync();
                    if (file != null)
                    {
                        var processedFile = await PrepareFile(file, cache, readContent);
                        tcs.SetResult(processedFile);
                    }
                    else
                    {
                        tcs.SetResult(new JSValueObject());
                    }
                });

                var result = await tcs.Task;

                List<JSValueObject> list = new List<JSValueObject>() { result };

                return list;
            }

            private async Task<List<JSValueObject>> PickFolderAsync(FolderPicker picker, bool cache, bool readContent)
            {
                TaskCompletionSource<List<JSValueObject>> tcs = new TaskCompletionSource<List<JSValueObject>>();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var folder = await picker.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        List<JSValueObject> jarrayObj = new List<JSValueObject>();
                        var files = await folder.GetFilesAsync();
                        foreach (var file in files)
                        {
                            var preparedFile = await PrepareFile(file, cache, readContent);
                            jarrayObj.Add(preparedFile);
                        }

                        tcs.SetResult(jarrayObj);
                    }
                });

                var result = await tcs.Task;
                return result;
            }
        }
    }





