using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    public sealed unsafe class BattleDispatcher
    {
        private static SC_CombatListEntry* g_enemiesCombatList => (SC_CombatListEntry*) 0x004C9100;
        private static SC_CombatListEntry* g_arrayOf3CombatEntries => (SC_CombatListEntry*) 0x004BF040;
        private static SC_CombatListEntry* g_alliesCombatList_ => (SC_CombatListEntry*) 0x004C13F8;

        private static Int32 dword_0x4A2968
        {
            get => *((Int32*) 0x4A2968);
            set => *((Int32*) 0x4A2968) = value;
        }

        private static UInt32 g_SelectedCharacterInBattle
        {
            get => *((UInt32*) 0x004A2678);
            set => *((UInt32*) 0x004A2678) = value;
        }

        private static Int32 g_enemiesCombatListCount
        {
            get => *((Int32*) 0x004AF8C4);
            set => *((Int32*) 0x004AF8C4) = value;
        }

        private static Int32 g_alliesCombatListCount
        {
            get => *((Int32*) 0x004AF8C8);
            set => *((Int32*) 0x004AF8C8) = value;
        }

        private static Int32 IsAtbDisabled
        {
            get => *((Int32*) 0x004D0814);
            set => *((Int32*) 0x004D0814) = value;
        }

        private static Int32 gSelectedPlayerActor
        {
            get => *((Int32*) 0x004DE624);
            set => *((Int32*) 0x004DE624) = value;
        }

        private static Int32 gBattlePartySize
        {
            get => *((Int32*) 0x004AF8CC);
            set => *((Int32*) 0x004AF8CC) = value;
        }

        private static Int32 DesiredActorToSelect
        {
            get => *((Int32*) 0x004A295C);
            set => *((Int32*) 0x004A295C) = value;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate SceneInstance* sub_407B70_(int value);
        private static readonly sub_407B70_ sub_407B70 = Marshal.GetDelegateForFunctionPointer<sub_407B70_>(new IntPtr(0x407B70));

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int IncreaseActorBattleTime_(ActorInfo2* a1);
        private static readonly IncreaseActorBattleTime_ IncreaseActorBattleTime = Marshal.GetDelegateForFunctionPointer<IncreaseActorBattleTime_>(new IntPtr(0x442310));

        private Int32 _selectAgainPlayerIndex = -1;
        private Boolean _forceDispatch = false;
        private Boolean _allPlayersIsFull = false;
        private Int32 _minEnemyAtb = 0;
        private Boolean _isEnemyMoved;

        public unsafe Boolean Dispatch()
        {
            if (_selectAgainPlayerIndex > -1)
            {
                DesiredActorToSelect = _selectAgainPlayerIndex;
                _selectAgainPlayerIndex = -1;
                _forceDispatch = true;
                return false;
            }

            if (!HotKeyManager.TryEvict(HotKey.NextTurn) && !_forceDispatch)
                return false;

            _forceDispatch = false;

            if (IsAtbDisabled != 0)
                return false;

            var partySize = gBattlePartySize;
            if (partySize < 1)
                return false;

            var enemySize = g_enemiesCombatListCount;
            if (enemySize < 1)
                return false;

            var otherSize = g_alliesCombatListCount;

            Boolean hasNext = false;
            Boolean hasChanged = false;

            do
            {
                if (!TryUpdatePlayerTimers(partySize, ref hasNext, ref hasChanged))
                    return false;

                if (!TryUpdateEnemyTimers(enemySize, ref hasNext, ref hasChanged))
                    return false;

                TryUpdateOtherTimers(otherSize);

                if (!hasNext)
                {

                }

            } while (hasNext && !hasChanged);

            return false;
        }

        private bool TryUpdatePlayerTimers(int partySize, ref bool hasNext, ref bool hasChanged)
        {
            _allPlayersIsFull = true;
            
            var disabledParty = 0;
            var maxAtb = 0;

            for (var i = 0; i < partySize; i++)
            {
                SC_CombatListEntry* playerCombatant = g_arrayOf3CombatEntries + i;

                var playerActor = playerCombatant->Actor;
                var isDisabled = IsActorDisabled(playerActor);

                var previousAtb = playerActor->Battle.ATB / 3333;
                if (isDisabled)
                {
                    disabledParty++;
                }
                else if (previousAtb < 3)
                {
                    _allPlayersIsFull = false;
                }

                IncreaseActorBattleTime(playerActor);

                if (isDisabled)
                    continue;

                var currentAtb = playerActor->Battle.ATB / 3333;
                if (currentAtb != 3)
                    hasNext = true;

                if (currentAtb > maxAtb)
                {
                    maxAtb = currentAtb;
                    if (!hasChanged && gSelectedPlayerActor == -1)
                        DesiredActorToSelect = i;
                }

                if (previousAtb != currentAtb)
                {
                    DesiredActorToSelect = i;
                    hasChanged = true;
                }
            }

            if (disabledParty == partySize)
                return false;
            return true;
        }

        private Boolean TryUpdateEnemyTimers(Int32 enemySize, ref Boolean hasNext, ref Boolean hasChanged)
        {
            var minAtb = Int32.MaxValue;

            var disabledEnemy = 0;
            for (int i = 0; i < enemySize; i++)
            {
                SC_CombatListEntry* enemyCombatant = g_enemiesCombatList + i;

                if (!IsActorDisabled(enemyCombatant->Actor))
                {
                    var atb = enemyCombatant->Actor->Battle.ATB;
                    if (atb < minAtb)
                        minAtb = atb;
                    
                    var beforeAtb = atb / 3333;
                    IncreaseActorBattleTime(enemyCombatant->Actor);
                    var currentAtb = enemyCombatant->Actor->Battle.ATB / 3333;
                    if (currentAtb != 3 && _allPlayersIsFull)
                        hasNext = true;

                    if (beforeAtb != currentAtb)
                    {
                        ReselectCurrentPlayer();
                        hasChanged = true;
                    }
                }
                else
                {
                    disabledEnemy++;
                }
            }

            if (minAtb < _minEnemyAtb)
                _isEnemyMoved = true;

            _minEnemyAtb = minAtb;

            if (disabledEnemy == enemySize)
                return false;
            return true;
        }

        private void ReselectCurrentPlayer()
        {
            if (gSelectedPlayerActor == -1)
                return;

            _selectAgainPlayerIndex = gSelectedPlayerActor;
            g_SelectedCharacterInBattle = 0xFFFFFFFF;
            sub_407B70(1);
            dword_0x4A2968 = -1;
            gSelectedPlayerActor = -1;
            DesiredActorToSelect = -1;
        }

        private static void TryUpdateOtherTimers(int otherSize)
        {
            for (int i = 0; i < otherSize; i++)
            {
                var other = g_alliesCombatList_ + i;
                IncreaseActorBattleTime(other->Actor);
            }
        }

        private static bool IsActorDisabled(ActorInfo2* playerActor)
        {
            var f1 = playerActor->Battle.SomeFlags;
            var f2 = playerActor->SomeFlags;
            var isDisabled = (f1 & 0x10000) == 0 || (f2 & 0x8) == 0;
            return isDisabled;
        }
    }
}