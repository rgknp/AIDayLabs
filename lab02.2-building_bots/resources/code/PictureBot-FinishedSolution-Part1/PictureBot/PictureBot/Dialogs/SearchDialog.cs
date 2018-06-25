using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder.Dialogs;
using PictureBot.Responses;
using PictureBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace PictureBot.Dialogs
{
    public class SearchDialog : DialogContainer
    {
        public const string Id = "searchPictures";

        public static SearchDialog Instance { get; } = new SearchDialog();

        // You can start this from the parent using the dialog's ID.
        public SearchDialog() : base(Id)
        {
            // add search dialog contents here
        }
    }
}

