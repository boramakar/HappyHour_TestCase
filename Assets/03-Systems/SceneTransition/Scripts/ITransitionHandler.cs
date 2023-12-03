    using System;

    public interface ITransitionHandler
    {
        public void FadeOut(Action callback);
        public void FadeIn(Action callback);
    }