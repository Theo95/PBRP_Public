using System.Collections.Generic;

namespace PBRP
{
    public enum PedVariationData
    {
        PED_VARIATION_FACE = 0,
        PED_VARIATION_HEAD = 1,
        PED_VARIATION_HAIR = 2,
        PED_VARIATION_TORSO = 3,
        PED_VARIATION_LEGS = 4,
        PED_VARIATION_HANDS = 5,
        PED_VARIATION_FEET = 6,
        PED_VARIATION_EYES = 7,
        PED_VARIATION_ACCESSORIES = 8,
        PED_VARIATION_TASKS = 9,
        PED_VARIATION_TEXTURES = 10,
        PED_VARIATION_TORSO2 = 11
    };

    public class Skin
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int Model { get; set; }
        public string DrawableIds { get; set; }
        public string TextureIds { get; set; }

        public List<SkinVariations> Variations()
        {
            List<SkinVariations> variants = new List<SkinVariations>();
            for (int i = 0; i < 12; i++)
            {
                variants.Add(new SkinVariations
                {
                    Drawable = int.Parse(DrawableIds.Split(',')[i]),
                    Texture = int.Parse(TextureIds.Split(',')[i])
                });
            }
            return variants;
        }
    }

    public class SkinVariations
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }
    }
}
