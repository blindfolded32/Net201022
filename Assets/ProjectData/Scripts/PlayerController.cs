using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor.PackageManager;
using UnityEngine;

namespace ProjectData.Scripts
{
    public abstract class PlayerController: MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] protected CharacterController _characterController;
        [SerializeField] protected Camera _camera;
        [SerializeField] protected AudioListener _listner;
        [SerializeField] protected PhotonView _photonView;
        [SerializeField] protected float _speed;
        [SerializeField] protected float _sensetivity;
    
        [SerializeField] protected float _health;
        [SerializeField] protected float _damage;
        [SerializeField] protected GameObject _weapon;
    
        protected bool _isAttack;
        protected Vector3 _attackRotation = new Vector3(90,0,0);

        protected Vector3 _defaultRotation = new Vector3(0,0,0);
        public PhotonView PhotonView => _photonView;

        protected void Start()
        {
            if(!_photonView.IsMine)
            {
                _camera.enabled = false;
                _listner.enabled = false;
            }
        }

        protected void Update()
        {
            if(_photonView.IsMine) 
            {
                Move();
                Attack();
            }
        }

        protected void FixedUpdate()
        {
            if (_photonView.IsMine)
            {
                Rotate();
            }
        }

        protected void Rotate()
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * _sensetivity * Time.fixedDeltaTime, 0);
        }

        protected void Move()
        {
            var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            _characterController.Move(move * Time.deltaTime * _speed);
        }
        protected void GetDamage(float damage)
        {
            _health -= damage;
            SendHpParameter(_photonView.ViewID, _health);
            Debug.Log($"{_photonView.Controller.UserId} health is {_health}");
        }

        protected void Attack()
        {
            if (_photonView.IsMine)
            {

                if (Input.GetKey(KeyCode.Mouse0) && !_isAttack)
                {
                    _weapon.transform.rotation = Quaternion.Euler(_attackRotation);
                    _isAttack = true;
                    SendAttackEvent(_photonView.ViewID);
                }
                else if (!Input.GetKey(KeyCode.Mouse0) && _isAttack)
                {
                    _isAttack = false;
                }

                if (Input.GetKeyUp(KeyCode.Mouse0) && _weapon.transform.rotation != Quaternion.Euler(_defaultRotation)) _weapon.transform.rotation = Quaternion.Euler(_defaultRotation);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            SendDamageEvent(gameObject.GetComponent<PhotonView>().ViewID, _damage);
        }
        protected void SendAttackEvent(int viewID)
        {
            ReceiverGroup receiverGroup = ReceiverGroup.All;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            object[] eventContent = new object[]
            {
                viewID,
            };

            PhotonNetwork.RaiseEvent((int)GameEvents.Attack, eventContent, options, sendOptions);
        }

        protected void SendDamageEvent(int damagedPlayerID, float dmg)
        {
            ReceiverGroup receiverGroup = ReceiverGroup.All;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            object[] eventContent = new object[]
            {
                damagedPlayerID,
                dmg
            };

            PhotonNetwork.RaiseEvent((int)GameEvents.DealDamage, eventContent, options, sendOptions);
            Debug.Log("SendDamageEvent");
        }

        private void SendHpParameter(int viewID, float hp)
        {
            ReceiverGroup receiverGroup = ReceiverGroup.All;
            RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            object[] eventContent = new object[]
            {
                viewID,
                hp
            };

            PhotonNetwork.RaiseEvent((int)GameEvents.ChangeHealth, eventContent, options, sendOptions);
            Debug.Log("SendHPEvent");
        }


        public void OnEvent(EventData photonEvent)
        {
            switch ((GameEvents)photonEvent.Code)
            {
                case GameEvents.SyncParam:

                    object[] parametersSyncData = (object[])photonEvent.CustomData;

                    if(_photonView.ViewID == (int)parametersSyncData[0])
                    {
                        _health = (float)parametersSyncData[1];
                        _damage = (float)parametersSyncData[2];
                    }
                    break;

                case GameEvents.Attack:

                    object[] attackEventData = (object[])photonEvent.CustomData;
                    break;

                case GameEvents.DealDamage:

                    object[] damageEventData = (object[])photonEvent.CustomData;

                    if (_photonView.ViewID == (int)damageEventData[0])
                    {
                        GetDamage((float)damageEventData[1]);
                    }
                    break;

                case GameEvents.ChangeHealth:

                    object[] changeHealthEventData = (object[])photonEvent.CustomData;

                    if (_photonView.ViewID == (int)changeHealthEventData[0])
                    {
                        _health = (float)changeHealthEventData[1];
                    }
                    break;

                default:
                    break;
            }
        }

        public void SetCharacterParameters(float hp, float damage)
        {
            _health = hp;
            _damage = damage;
        }

    }
}