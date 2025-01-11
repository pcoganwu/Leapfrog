using System.Collections.ObjectModel;

namespace Leapfrog.Core.Entities
{
    public class NotUploadedModel
    {
        public bool HasChangesUploaded { get; set; }

        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
        public string ViewName { get; set; } = string.Empty;
        public string LabelName { get; set; } = string.Empty;

        public static bool HasUnsavedChanges(ObservableCollection<NotUploadedModel> unsavedFields)
        {
            return unsavedFields.Count > 0;
        }
    }
}
