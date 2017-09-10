namespace More.Composition.Hosting
{
    using System;

    /// <summary>
    /// Represents the Search contract options for the <see cref="ContractSettings"/>.
    /// </summary>
    public class SearchContractOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchContractOptions"/> class.
        /// </summary>
        public SearchContractOptions()
        {
            PlaceholderText = string.Empty;
            SearchHistoryContext = string.Empty;
            SearchHistoryEnabled = true;
        }

        /// <summary>
        /// Gets or sets the placeholder text in the search box when the search box doesn't have the input focus and no characters have been entered.
        /// </summary>
        /// <value>The placeholder text to display in the search box.</value>
        public string PlaceholderText { get; set; }

        /// <summary>
        /// Gets or sets a string that identifies the context of the search and is used to store the search history with the application.
        /// </summary>
        /// <value>The search history context string.</value>
        public string SearchHistoryContext { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether previous searches with the application are automatically tracked and used to provide suggestions.
        /// </summary>
        /// <value>True if the search search history is automatically tracked and used to provide suggestions; otherwise false. The default value is true.</value>
        public bool SearchHistoryEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the search pane can open by typing.
        /// </summary>
        /// <value>True if the search pane can open by typing; otherwise false. The default value is false.</value>
        public bool ShowOnKeyboardInput { get; set; }
    }
}