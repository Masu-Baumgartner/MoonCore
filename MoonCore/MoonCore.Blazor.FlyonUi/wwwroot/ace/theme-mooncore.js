define("ace/theme/mooncore-css",["require","exports","module"],function(e,t,n){
    n.exports = `
.ace-mooncore .ace_gutter {
  background: var(--mooncore-color-base-200);
  color: var(--mooncore-color-neutral);
}

.ace-mooncore .ace_print-margin {
  width: 1px;
  background: var(--mooncore-color-base-300);
}

.ace-mooncore {
  background-color: var(--mooncore-color-base-100);
  color: var(--mooncore-color-base-content);
}

.ace-mooncore .ace_constant.ace_other,
.ace-mooncore .ace_cursor {
  color: var(--mooncore-color-base-content);
}

.ace-mooncore .ace_marker-layer .ace_selection {
  background: var(--mooncore-color-primary);
}

.ace-mooncore.ace_multiselect .ace_selection.ace_start {
  box-shadow: 0 0 3px 0px var(--mooncore-color-base-100);
}

.ace-mooncore .ace_marker-layer .ace_step {
  background: var(--mooncore-color-warning);
}

.ace-mooncore .ace_marker-layer .ace_bracket {
  margin: -1px 0 0 -1px;
  border: 1px solid var(--mooncore-color-neutral-content);
}

.ace-mooncore .ace_marker-layer .ace_active-line {
  background: var(--mooncore-color-base-150);
}

.ace-mooncore .ace_gutter-active-line {
  background-color: var(--mooncore-color-base-200);
}

.ace-mooncore .ace_marker-layer .ace_selected-word {
  border: 1px solid var(--mooncore-color-primary);
}

.ace-mooncore .ace_invisible {
  color: var(--mooncore-color-neutral-content);
}

.ace-mooncore .ace_keyword,
.ace-mooncore .ace_meta,
.ace-mooncore .ace_storage,
.ace-mooncore .ace_storage.ace_type,
.ace-mooncore .ace_support.ace_type {
  color: var(--mooncore-color-accent);
}

.ace-mooncore .ace_keyword.ace_operator {
  color: var(--mooncore-color-info);
}

.ace-mooncore .ace_constant.ace_character,
.ace-mooncore .ace_constant.ace_language,
.ace-mooncore .ace_constant.ace_numeric,
.ace-mooncore .ace_keyword.ace_other.ace_unit,
.ace-mooncore .ace_support.ace_constant,
.ace-mooncore .ace_variable.ace_parameter {
  color: var(--mooncore-color-warning);
}

.ace-mooncore .ace_invalid {
  color: var(--mooncore-color-base-content);
  background-color: var(--mooncore-color-error);
}

.ace-mooncore .ace_invalid.ace_deprecated {
  color: var(--mooncore-color-base-content);
  background-color: var(--mooncore-color-warning);
}

.ace-mooncore .ace_fold {
  background-color: var(--mooncore-color-info);
  border-color: var(--mooncore-color-base-content);
}

.ace-mooncore .ace_entity.ace_name.ace_function,
.ace-mooncore .ace_support.ace_function,
.ace-mooncore .ace_variable {
  color: var(--mooncore-color-info);
}

.ace-mooncore .ace_support.ace_class,
.ace-mooncore .ace_support.ace_type {
  color: var(--mooncore-color-secondary);
}

.ace-mooncore .ace_heading,
.ace-mooncore .ace_markup.ace_heading,
.ace-mooncore .ace_string {
  color: var(--mooncore-color-success);
}

.ace-mooncore .ace_entity.ace_name.ace_tag,
.ace-mooncore .ace_entity.ace_other.ace_attribute-name,
.ace-mooncore .ace_meta.ace_tag,
.ace-mooncore .ace_string.ace_regexp,
.ace-mooncore .ace_variable {
  color: var(--mooncore-color-error);
}

.ace-mooncore .ace_comment {
  color: var(--mooncore-color-neutral);
}

.ace-mooncore .ace_indent-guide {
  background: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAACCAYAAACZgbYnAAAAEklEQVQImWNgYGBgYJDzqfwPAANXAeNsiA+ZAAAAAElFTkSuQmCC) right repeat-y;
}

.ace-mooncore .ace_indent-guide-active {
  background: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAACCAYAAACZgbYnAAAAEklEQVQIW2PQ1dX9zzBz5sz/ABCcBFFentLlAAAAAElFTkSuQmCC) right repeat-y;
}

.ace-mooncore .ace_constant.ace_buildin {
  color: var(--mooncore-color-primary);
}

.ace-mooncore .ace_variable.ace_language {
  color: var(--mooncore-color-base-content);
}
`;
});

define("ace/theme/mooncore",["require","exports","module","ace/theme/mooncore-css","ace/lib/dom"],function(e,t,n){
    t.isDark = !0;
    t.cssClass = "ace-mooncore";
    t.cssText = e("./mooncore-css");
    var r = e("../lib/dom");
    r.importCssString(t.cssText, t.cssClass, !1);
});

(function() {
    window.require(["ace/theme/mooncore"], function(m) {
        if (typeof module == "object" && typeof exports == "object" && module) {
            module.exports = m;
        }
    });
})();
