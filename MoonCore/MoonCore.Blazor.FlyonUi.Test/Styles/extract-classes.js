// extract-classes.js
const fs = require('fs');

module.exports = (opts = {}) => {
    const classSet = new Set();

    return {
        postcssPlugin: 'extract-tailwind-classes',
        Rule(rule) {
            const selectorParser = require('postcss-selector-parser');
            selectorParser(selectors => {
                selectors.walkClasses(node => {
                    classSet.add(node.value);
                });
            }).processSync(rule.selector);
        },
        OnceExit() {
            const classArray = Array.from(classSet).sort();
            fs.writeFileSync('../../MoonCore.Blazor.FlyonUi/Styles/mappings/mooncore.map', classArray.join('\n'));
            console.log(`✅ Extracted ${classArray.length} Tailwind classes to tailwind-classes.txt`);
        }
    };
};

module.exports.postcss = true;