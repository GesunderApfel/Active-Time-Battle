using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The heart of the prototype.
/// 
/// 
/// </summary>
public class BattleActionProcessor
{
    private BattleRuleBook _battleRuleBook;
    public BattleParties battleParties;

    /// <summary>
    /// Queue for all action coroutines
    /// </summary>
    private Queue<IEnumerator> _battleActions;
    /// <summary>
    /// Queue for all fightes which have a full turn meter
    /// </summary>
    private Queue<Fighter> _activeFighters;
    public Fighter ActiveFighter
    {
        get
        {
            if (_activeFighters.Count > 0)
                return _activeFighters.Peek();
            else
                return null;
        }
    }
    public Fighter LastActiveFighter;

    public int ActiveFighterIndex
    {
        get { return battleParties.GetIndexOfFighterInPlayerParty(ActiveFighter); }
    }

    public int LastActiveFighterIndex
    {
        get { return battleParties.GetIndexOfFighterInPlayerParty(LastActiveFighter); }
    }

    /// <summary>
    /// helper object for starting and stopping coroutines, because BattleActionProcessor is not MonoBehaviour
    /// </summary>
    private MonoBehaviour _coroutineStarter;
    private bool _isBattleFinished;
    private bool _isAnimationOnGoing;

    #region Controller Event Members
    // this region holds data relevant for the Controller / UI

    public Transform LastTargetTransform { get; private set; }
    public int LastDamageDealt { get; private set; }
    #endregion

    #region Controller Events
    // this region holds data relevant for the Controller / UI

    public event EventHandler onDamageDealt;
    public event EventHandler onBattleWon;
    public event EventHandler onBattleLost;

    private void DealDamage(Fighter target, int damage)
    {
        LastTargetTransform = battleParties.fighterTransformDict[target];
        LastDamageDealt = damage;
        onDamageDealt?.Invoke(this, EventArgs.Empty);
    }

    private void Win()
    {
        onBattleWon?.Invoke(this, EventArgs.Empty);
        StopBattle();
    }

    private void Lose()
    {
        onBattleLost?.Invoke(this, EventArgs.Empty);
        StopBattle();
    }

    private void StopBattle()
    {
        _isBattleFinished = true;
        _coroutineStarter.StopAllCoroutines();
        _battleActions.Clear();
    }
    #endregion

    public BattleActionProcessor(BattleParties battleParties)
    {
        _battleRuleBook = new BattleRuleBook();
        _activeFighters = new Queue<Fighter>();

        _isBattleFinished = false;
        _isAnimationOnGoing = false;


        this.battleParties = battleParties;
    }

    public void AddAttack(Fighter attacker, Fighter target)
    {
        _battleActions.Enqueue(AttackCoroutine(attacker, battleParties.fighterTransformDict[attacker], target));
    }

    public void AddMagic(Fighter attacker, Fighter target, Magic spell)
    {
        _battleActions.Enqueue(MagicCoroutine(attacker, battleParties.fighterTransformDict[attacker], target, spell));
    }

    public void AddBestia(Fighter attacker, Fighter target, Bestia bestia)
    {
        _battleActions.Enqueue(BestiaCoroutine(attacker, battleParties.fighterTransformDict[attacker], target, bestia));
    }

    public void EndActiveFighterTurn()
    {
        LastActiveFighter = _activeFighters.Dequeue();
        LastActiveFighter.isDoingAction = true;
    }

    #region Coroutines

    /// <summary>
    /// courotineStarter needs to be a MonoBehaviour rooted in the scene. In this case it's simply the BattleMenuController.
    /// This is needed, because BattleMenuController isn't a MonoBehaviour.
    /// </summary>
    public void StartCoroutines(MonoBehaviour coroutineStarter)
    {
        _coroutineStarter = coroutineStarter;
        _coroutineStarter.StartCoroutine(BattleActionsQueueHandler());
        _coroutineStarter.StartCoroutine(FighterTurnMeter());
    }

    /// <summary>
    /// Regulates invoking of registered actions (animation coroutines and calculation logic)
    /// by invoking them one after another (after one animation cycle is done)
    /// </summary>
    /// <returns></returns>
    public IEnumerator BattleActionsQueueHandler()
    {
        _battleActions = new Queue<IEnumerator>();
        _isAnimationOnGoing = false;
        while (!_isBattleFinished)
        {
            if (_isAnimationOnGoing)
                continue;

            if (_battleActions.Count > 0)
            {
                yield return _coroutineStarter.StartCoroutine(_battleActions.Dequeue());
            }
            yield return null;
        }
    }

    /// <summary>
    /// Handles the turn meter bar and fighter turns.
    /// </summary>
    public IEnumerator FighterTurnMeter()
    {
        while (!_isBattleFinished)
        {
            foreach (var fighter in battleParties.allFighters)
            {
                // dont add fighter whose turn ended (isDoingAction) / who is already queued
                if (fighter.isDoingAction || _activeFighters.Contains(fighter))
                    continue;

                fighter.attackTurnMeter += fighter.speed * Time.deltaTime;
                if (battleParties.PlayerParty.Fighters.Contains(fighter))
                {
                    battleParties.fighterTurnMeterDict[fighter].UpdateTurnMeter(fighter.attackTurnMeter);
                    if (fighter.attackTurnMeter >= 100f)
                    {
                        _activeFighters.Enqueue(fighter);
                    }
                }

                /*
                 * Enemy not implemented
                 */
            }
            yield return null;
        }
    }

    IEnumerator AttackCoroutine(Fighter attacker, Transform attackerTransform, Fighter target)
    {
        target = ValidateCurrentTarget(target);

        if (target != null && !_isBattleFinished)
        {
            _isAnimationOnGoing = true;

            Transform targetTransform = battleParties.fighterTransformDict[target];

            Vector3 standingPos = attackerTransform.position;
            Quaternion originalRotation = attackerTransform.rotation;
            Sequence attackSequence = DOTween.Sequence();

            attackSequence.Append(attackerTransform.DOLookAt(targetTransform.position, 0.1f));
            attackSequence.Append(attackerTransform.DOMove(targetTransform.position - Vector3.right, 1));
            attackSequence.Append(targetTransform.DOShakePosition(2));
            attackSequence.Join(attackerTransform.DOShakePosition(2, fadeOut: false));
            attackSequence.Append(attackerTransform.DOJump(standingPos, 3, 1, 1));


            yield return attackSequence.WaitForElapsedLoops(1);
            attackerTransform.rotation = originalRotation;
            attacker.isDoingAction = false;
            attacker.attackTurnMeter = 0;

            DealDamage(target, _battleRuleBook.DoDamage(attacker, target));
            TargetDying(target);

            _isAnimationOnGoing = false;
        }

        CheckBattleState();
    }

    IEnumerator MagicCoroutine(Fighter attacker, Transform attackerTransform, Fighter target, Magic magic)
    {
        target = ValidateCurrentTarget(target);

        if (target != null && !_isBattleFinished)
        {
            _isAnimationOnGoing = true;

            Transform targetTransform = battleParties.fighterTransformDict[target];

            Vector3 standingPos = attackerTransform.position;
            Quaternion originalRotation = attackerTransform.rotation;
            Sequence attackSequence = DOTween.Sequence();

            attackSequence.Append(attackerTransform.DOLookAt(targetTransform.position, 0.1f));
            attackSequence.Append(attackerTransform.DOShakePosition(2));
            yield return attackSequence.WaitForElapsedLoops(1);

            /// just a note for myself ^^
            /// TO DO
            /// loop through all hit times
            /// When TO DO: after "real" animations got implemented

            GameObject magicParent = GameObject.Instantiate(magic.effect, targetTransform.position, Quaternion.identity);
            MagicTimer timer = magicParent.GetComponent<MagicTimer>();
            float waitTime = timer.durationUntilHit[0];
            yield return new WaitForSeconds(waitTime);

            attackSequence = DOTween.Sequence();
            attackSequence.Append(targetTransform.DOShakePosition(1));
            yield return new WaitForSeconds(timer.endTime - waitTime);

            GameObject.Destroy(magicParent);
            attackerTransform.rotation = originalRotation;
            attacker.isDoingAction = false;
            attacker.attackTurnMeter = 0;


            DealDamage(target, _battleRuleBook.DoSubMenuAction(attacker, target, magic));
            TargetDying(target);

            _isAnimationOnGoing = false;
        }

        CheckBattleState();
    }

    IEnumerator BestiaCoroutine(Fighter attacker, Transform attackerTransform, Fighter target, Bestia bestia)
    {
        target = ValidateCurrentTarget(target);

        if (target != null && !_isBattleFinished)
        {
            _isAnimationOnGoing = true;

            Transform targetTransform = battleParties.fighterTransformDict[target];

            Vector3 standingPos = attackerTransform.position;
            Quaternion originalRotation = attackerTransform.rotation;
            Sequence attackSequence = DOTween.Sequence();

            attackSequence.Append(attackerTransform.DOLookAt(targetTransform.position, 0.1f));
            attackSequence.Append(attackerTransform.DOShakePosition(2));
            yield return attackSequence.WaitForElapsedLoops(1);

            /// just a note for myself ^^
            /// TO DO
            /// loop through all hit times
            /// When TO DO: after "real" animations got implemented

            Vector3 spawnPosition = targetTransform.position - Vector3.up * targetTransform.localScale.y * 0.5f;
            GameObject bestiaCallGO = GameObject.Instantiate(bestia.bestiaCall, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(bestia.duration);

            attackerTransform.rotation = originalRotation;
            attacker.isDoingAction = false;
            attacker.attackTurnMeter = 0;


            DealDamage(target, _battleRuleBook.DoSubMenuAction(attacker, target, bestia));
            TargetDying(target);

            _isAnimationOnGoing = false;
        }

        CheckBattleState();
    }

    /// <summary>
    /// Checks if battle is over and which team won.
    /// There is no draw and loss of the player has priority
    /// </summary>
    private void CheckBattleState()
    {
        var playerFighterAlive = battleParties.PlayerParty.Fighters.FirstOrDefault(x => x.health > 0);
        if (playerFighterAlive == null)
            Lose();

        var enemyFighterAlive = battleParties.EnemyParty.Fighters.FirstOrDefault(x => x.health > 0);
        if (enemyFighterAlive == null)
            Win();
    }

    /// <summary>
    /// checks if target is still alive, if not -> returns next target
    /// if all "targets" are dead -> lose or win
    /// </summary>
    private Fighter ValidateCurrentTarget(Fighter target)
    {
        if (target.health <= 0)
        {
            if (battleParties.IsFighterInPlayerParty(target))
            {
                foreach (Fighter member in battleParties.PlayerParty.Fighters)
                {
                    if (member.health > 0)
                    {
                        return member;
                    }
                }
                return null;
            }
            else
            {
                if (battleParties.fighterTransformDict.Keys.Contains(target))
                    battleParties.fighterTransformDict.Remove(target);

                foreach (Fighter member in battleParties.EnemyParty.Fighters)
                {
                    if (member.health > 0)
                    {
                        return member;
                    }
                }
                return null;
            }
        }

        return target;
    }

    public void TargetDying(Fighter target)
    {
        if (target.health <= 0)
        {
            battleParties.KillFighter(target);
        }
    }
    #endregion
}

