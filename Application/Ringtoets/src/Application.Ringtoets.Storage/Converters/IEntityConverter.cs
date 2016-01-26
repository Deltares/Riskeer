namespace Application.Ringtoets.Storage.Converters
{
    public interface IEntityConverter<T1, T2>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="T1"/>.
        /// </summary>
        /// <param name="entity">The <see cref="T2"/> to convert.</param>
        /// <returns>A new instance of <see cref="T1"/>, based on the properties of <paramref name="entity"/>.</returns>
        T1 ConvertEntityToModel(T2 entity);

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="T1"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="T2"/> to be saved.</param>
        void ConvertModelToEntity(T1 modelObject, T2 entity);
    }
}