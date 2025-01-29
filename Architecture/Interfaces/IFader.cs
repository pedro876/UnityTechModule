using System.Collections;

namespace Architecture
{
    public interface IFader
    {
        public IEnumerator CO_FadeIn(float time);
        public IEnumerator CO_FadeOut(float time);
    }
}
