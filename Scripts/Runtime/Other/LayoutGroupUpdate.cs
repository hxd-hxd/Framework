using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 处理 <see cref="LayoutGroup"/> 组件初始关闭，第一次激活不及时更新计算布局的问题
    /// <para>将次组件挂在 <see cref="LayoutGroup"/> 同物体上即可</para>
    /// </summary>
    public class LayoutGroupUpdate : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;
            var lg = GetComponent<LayoutGroup>();
            if (lg != null)
            {
                if (lg.enabled)
                {
                    lg.enabled = false;
                    yield return null;
                    lg.enabled = true;
                }
            }
        }

        private void OnEnable()
        {
            //StartCoroutine(UpdateLayoutGroup());
        }

        private void OnDisable()
        {
            //StopAllCoroutines();
        }

        private IEnumerator UpdateLayoutGroup()
        {
            var lg = GetComponent<LayoutGroup>();

            while (true)
            {
                if (lg != null)
                {
                    if (lg.enabled)
                    {
                        lg.enabled = false;
                        yield return null;
                        lg.enabled = true;
                        yield return null;
                    }
                }
            }
        }
    }
}

