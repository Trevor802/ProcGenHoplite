public class Archer : Enemy{
    protected override void Awake() {
        base.Awake();
        m_isArcher = true;
    }
}