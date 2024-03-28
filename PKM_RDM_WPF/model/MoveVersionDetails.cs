namespace PKM_RDM_WPF.model
{
    public class MoveVersionDetails
    {
        private int level_learned_at;
        private NameUrl move_learn_method;
        private NameUrl version_group;

        public MoveVersionDetails(){}

        public MoveVersionDetails(int level_learned_at, NameUrl move_learn_method, NameUrl version_group)
        {
            this.Level_learned_at = level_learned_at;
            this.Move_learn_method = move_learn_method;
            this.Version_group = version_group;
        }

        public int Level_learned_at { get => level_learned_at; set => level_learned_at = value; }
        public NameUrl Move_learn_method { get => move_learn_method; set => move_learn_method = value; }
        public NameUrl Version_group { get => version_group; set => version_group = value; }
    }
}