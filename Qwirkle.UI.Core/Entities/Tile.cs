using Qwirkle.Core.Enums;

namespace Qwirkle.Core.Entities
{
    public class Tile
    {
        public int Id { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }

        public Tile(int id, TileColor color, TileForm form)
        {
            Id = id;
            Color = color;
            Form = form;
        }

        public bool HaveFormOrColorOnlyEqual(Tile tile)
        {
            if (Color == tile.Color && Form != tile.Form || Color != tile.Color && Form == tile.Form) return true;
            return false;
        }

        public string GetNameImage()
        {
            string name = Color.ToString() + Form.ToString() + ".png";
            return name;
        }
    }
}
