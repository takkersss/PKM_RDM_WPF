namespace PKM_RDM_WPF.model
{
    public class Sprite
    {
        private string back_default, back_female, back_shiny, back_shiny_female,
            front_default, front_female, front_shiny, front_shiny_female;

        public Sprite() { }

        public string Back_default { get => back_default; set => back_default = value; }
        public string Back_female { get => back_female; set => back_female = value; }
        public string Back_shiny { get => back_shiny; set => back_shiny = value; }
        public string Back_shiny_female { get => back_shiny_female; set => back_shiny_female = value; }
        public string Front_default { get => front_default; set => front_default = value; }
        public string Front_female { get => front_female; set => front_female = value; }
        public string Front_shiny { get => front_shiny; set => front_shiny = value; }
        public string Front_shiny_female { get => front_shiny_female; set => front_shiny_female = value; }
    }
}