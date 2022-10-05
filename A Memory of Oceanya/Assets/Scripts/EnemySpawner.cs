using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : ObjectList
{
    public List<Character> enemies { get; private set; }

    public BattleUIList uiList;

    public static EnemySpawner instance { get; private set; }

    private void Start()
    {
        instance = this;
    }

    public void SpawnAllEnemies(List<Character> enemyList)
    {
        ClearList();

        enemies = enemyList;

        int totalEnemies = enemyList.Count;

        float posX = 1f / (totalEnemies + 1f);

        for (int i = 0; i < totalEnemies; i++)
        {
            Vector3 newPos = new Vector3(posX * (i + 1), 0, 0);

            GameObject curEnemy = AddObject(newPos, new Quaternion(0,0,0,0));
            curEnemy.transform.localPosition = newPos;

            EffectAnimator curEffectAnimator = curEnemy.GetComponentInChildren<EffectAnimator>();
            SpriteAnimator curSpriteAnimator = curEnemy.GetComponentInChildren<SpriteAnimator>();
            EnemySprite curSprite = curEnemy.GetComponentInChildren<EnemySprite>();
            curSprite.SwitchSprites(enemyList[i].view.sprite);

            Transform[] children = curEnemy.GetComponentsInChildren<Transform>();
            for (int j = 0; j < children.Length; j++)
            {
                if (children[j].gameObject.CompareTag("UIPlacement"))
                {
                    EnemyBattleUI newBattleUI = uiList.AddEnemy(enemyList[i], children[j]);
                    newBattleUI.effectAnimator = curEffectAnimator;
                    newBattleUI.effectAnimator.ui = newBattleUI;
                    enemyList[i].view.curSprite = curSpriteAnimator;
                    break;
                }
            }
        }
    }
}
