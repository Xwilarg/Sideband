using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Particles To Band")]
    public class ParticlesToBand : AudioBandListenerExtension
    {
        [Space]
        [Tooltip("Buffered values provide smoother falloff of values")]
        [SerializeField] private bool UseBufferedValues = true;
        [Tooltip("Additional smoothing")]
        [SerializeField] private float LerpSpeed = 8;

        [Header("Base Settings")]
        [SerializeField] private bool AffectBaseSettings;
        [SerializeField] private MinMaxFloat StartLifetime;
        [SerializeField] private MinMaxFloat StartSpeed;
        [SerializeField] private MinMaxFloat StartSize;
        [SerializeField] private MinMaxFloat StartRotation;
        [SerializeField] private MinMaxColor StartColor;
        [SerializeField] private MinMaxFloat GravityModifier;

        [Header("Emission")]
        [SerializeField] private bool AffectEmission;
        [SerializeField] private MinMaxFloat EmissionRateOverTime;
        [SerializeField] private MinMaxFloat EmissionRateOverDistance;

        [Header("Shape")]
        [SerializeField] private bool AffectShape;
        [SerializeField] private MinMaxVector3 ShapePosition;
        [SerializeField] private MinMaxVector3 ShapeRotation;
        [SerializeField] private MinMaxVector3 ShapeScale;

        [Header("Runtime")]
        [SerializeField] private bool AffectRuntime;
        [SerializeField] private MinMaxFloat ParticleSize;
        [SerializeField] private MinMaxFloat ParticleRotation;
        [SerializeField] private MinMaxColor ParticleColor;

        private ParticleSystem _particleSystem;
        private float _currentBandValue;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!AudioBandListener.IsPlaying) return;

            var band = AudioBandListener.Band(Band, UseBufferedValues);
            _currentBandValue = LerpSpeed == 0 ? band : Mathf.Lerp(_currentBandValue, band, Time.deltaTime * LerpSpeed);

            if (AffectBaseSettings)
            {
                var main = _particleSystem.main;
                main.startLifetime = StartLifetime.Lerp(_currentBandValue);
                main.startSpeed = StartSpeed.Lerp(_currentBandValue);
                main.startSize = StartSize.Lerp(_currentBandValue);
                main.startRotation = StartRotation.Lerp(_currentBandValue);
                main.startColor = StartColor.Lerp(_currentBandValue);
                main.gravityModifier = GravityModifier.Lerp(_currentBandValue);
            }

            if (AffectEmission)
            {
                var emission = _particleSystem.emission;
                emission.rateOverTime = EmissionRateOverTime.Lerp(_currentBandValue);
                emission.rateOverDistance = EmissionRateOverDistance.Lerp(_currentBandValue);
            }

            if (AffectShape)
            {
                var shape = _particleSystem.shape;
                shape.position = ShapePosition.Lerp(_currentBandValue);
                shape.rotation = ShapeRotation.Lerp(_currentBandValue);
                shape.scale = ShapeScale.Lerp(_currentBandValue);
            }

            if (AffectRuntime)
            {
                var particles = new ParticleSystem.Particle[_particleSystem.particleCount];
                _particleSystem.GetParticles(particles);

                for (var i = 0; i < particles.Length; i++)
                {
                    var particle = particles[i];

                    if (AffectRuntime)
                    {
                        particle.startSize = ParticleSize.Lerp(_currentBandValue);
                        particle.rotation = ParticleRotation.Lerp(_currentBandValue);
                        particle.startColor = ParticleColor.Lerp(_currentBandValue);
                    }

                    particles[i] = particle;
                }

                _particleSystem.SetParticles(particles);
            }
        }

        [Serializable]
        private struct MinMaxFloat
        {
            public float Min;
            public float Max;

            public float Lerp(float t) => Mathf.Lerp(Min, Max, t);
        }

        [Serializable]
        private struct MinMaxColor
        {
            public Color Min;
            public Color Max;

            public Color Lerp(float t) => Color.Lerp(Min, Max, t);
        }

        [Serializable]
        private struct MinMaxVector3
        {
            public Vector3 Min;
            public Vector3 Max;

            public Vector3 Lerp(float t) => Vector3.Lerp(Min, Max, t);
        }

        [Serializable]
        private struct MinMaxVector2
        {
            public Vector2 Min;
            public Vector2 Max;

            public Vector2 Lerp(float t) => Vector2.Lerp(Min, Max, t);
        }
    }
}
