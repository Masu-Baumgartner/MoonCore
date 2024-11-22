npx tw-patch install
npx tw-patch extract
cp .tw-patch/tw-class-list.json ../../MoonCore.Blazor.Tailwind/Styles/mappings/mooncore.map
rm -r .\.tw-patch\