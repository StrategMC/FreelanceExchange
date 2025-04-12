namespace FreelanceBirga.Models.VM
{
   
        public class SearchViewModel
        {
            public List<TagViewModel> AllTags { get; set; }
            public List<ExecutorViewModel> FilteredExecutors { get; set; }
            public List<int>? SelectedTagIds { get; set; }
        }
    
}
