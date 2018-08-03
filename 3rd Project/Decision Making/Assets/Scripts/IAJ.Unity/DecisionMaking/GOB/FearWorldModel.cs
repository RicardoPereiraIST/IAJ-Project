using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class FearWorldModel : WorldModel
    {
        private object[] Properties { get; set; }

        public FearWorldModel(List<Action> actions, GameManager.GameManager gameManager) : base(actions)
        {
            this.Properties = new object[22];
            InitializeData(gameManager);
        }

        public FearWorldModel(FearWorldModel parent) : base (parent)
        {
            this.Properties = (object[])parent.Properties.Clone();
        }

        public override void InitializeCharacter(GameManager.GameManager gameManager)
        {
            Properties[0] = gameManager.characterData.CharacterGameObject.transform.position;
            Properties[1] = gameManager.characterData.Time;
            Properties[2] = gameManager.characterData.Mana;
            Properties[5] = gameManager.characterData.HP;
            Properties[8] = gameManager.characterData.XP;
            Properties[14] = gameManager.characterData.Money;
            Properties[20] = gameManager.characterData.MaxHP;
            Properties[21] = gameManager.characterData.Level;
        }

        private void InitializeData(GameManager.GameManager gameManager)
        {
            Properties[0] = gameManager.characterData.CharacterGameObject.transform.position;
            Properties[1] = gameManager.characterData.Time;
            Properties[2] = gameManager.characterData.Mana;
            Properties[3] = true;
            Properties[4] = true;
            Properties[5] = gameManager.characterData.HP;
            Properties[6] = true;
            Properties[7] = true;
            Properties[8] = gameManager.characterData.XP;
            Properties[9] = true;
            Properties[10] = true;
            Properties[11] = true;
            Properties[12] = true;
            Properties[13] = true;
            Properties[14] = gameManager.characterData.Money;
            Properties[15] = true;
            Properties[16] = true;
            Properties[17] = true;
            Properties[18] = true;
            Properties[19] = true;
            Properties[20] = gameManager.characterData.MaxHP;
            Properties[21] = gameManager.characterData.Level;
        }

        public override object GetProperty(string propertyName)
        {
            int propertyIndex = GetPropertyIndex(propertyName);
            if (propertyIndex != -1)
            {
                return this.Properties[propertyIndex];
            }
            else
                return null;
        }

        private int GetPropertyIndex(string propertyName)
        {
            return DataManager.Instance.PropertiesNames[propertyName.GetHashCode()];
        }

        public override void SetProperty(string propertyName, object value)
        {
            int propertyIndex = GetPropertyIndex(propertyName);
            if(propertyIndex != -1)
                this.Properties[propertyIndex] = value;
        }

        public override WorldModel GenerateChildWorldModel()
        {
            return new FearWorldModel(this);
        }

        public override Action GetNextAction()
        {
            return base.GetNextAction();
        }

        public override Action[] GetExecutableActions()
        {
            return base.GetExecutableActions();
        }

        public override bool IsTerminal()
        {
            return base.IsTerminal();
        }


        public override float GetScore()
        {
            return base.GetScore();
        }

        public override int GetNextPlayer()
        {
            return base.GetNextPlayer();
        }

        public override void CalculateNextPlayer()
        {
        }
    }
}
