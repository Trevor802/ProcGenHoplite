public class Warrior : Enemy{
    protected override void Awake() {
        base.Awake();
        m_isArcher = false;
    }
}